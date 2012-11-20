using System.Threading;

using LibOfLegends;

using FluorineFx;
using FluorineFx.Net;

using com.riotgames.platform.statistics;
using com.riotgames.platform.gameclient.domain;

namespace RiotGear
{
	class ConcurrentRPC
	{
		const int RPCTimeout = 10000;

		RPCService RPC;
		int AccountId;

		AutoResetEvent RPCEvent;
		int Counter;
		bool ErrorOccurred;

		public AllPublicSummonerDataDTO PublicSummonerData;
		public PlayerLifeTimeStats LifeTimeStatistics;
		// One entry for each season
		public AggregatedStats[] AggregatedStatistics;
		public RecentGames RecentGameData;

		public ConcurrentRPC(RPCService rpc, int accountId)
		{
			RPC = rpc;
			AccountId = accountId;

			AggregatedStatistics = new AggregatedStats[StatisticsService.Seasons];
		}

		public OperationResult Run()
		{
			RPCEvent = new AutoResetEvent(false);
			ErrorOccurred = false;
			// Number of queries to perform in parallel
			Counter = 6;

			try
			{
				// The queries
				RPC.GetAllPublicSummonerDataByAccountAsync(AccountId, new Responder<AllPublicSummonerDataDTO>(GetPublicSummonerData, Error));
				RPC.RetrievePlayerStatsByAccountIDAsync(AccountId, "CURRENT", new Responder<PlayerLifeTimeStats>(GetLifeTimeStatistics, Error));
				RPC.GetAggregatedStatsAsync(AccountId, "CLASSIC", "CURRENT", new Responder<AggregatedStats>((AggregatedStats aggregatedStatistics) => GetAggregatedStatistics(aggregatedStatistics, 0), Error));
				RPC.GetAggregatedStatsAsync(AccountId, "CLASSIC", "ONE", new Responder<AggregatedStats>((AggregatedStats aggregatedStatistics) => GetAggregatedStatistics(aggregatedStatistics, 1), Error));
				RPC.GetAggregatedStatsAsync(AccountId, "CLASSIC", "TWO", new Responder<AggregatedStats>((AggregatedStats aggregatedStatistics) => GetAggregatedStatistics(aggregatedStatistics, 2), Error));
				RPC.GetRecentGamesAsync(AccountId, new Responder<RecentGames>(GetRecentGameData, Error));

				if(RPCEvent.WaitOne(RPCTimeout))
				{
					if (ErrorOccurred)
					{
						//This is not correct - it should really return some error state but I can't be bothered to change the JavaScript handling of error enums right now
						return OperationResult.NotFound;
					}
					else if (PublicSummonerData == null || LifeTimeStatistics == null || AggregatedStatistics == null || RecentGameData == null)
						return OperationResult.NotFound;
					else
						return OperationResult.Success;
				}
				else
					return OperationResult.Timeout;
			}
			catch (RPCNotConnectedException)
			{
				return OperationResult.NotConnected;
			}
		}

		void ProcessReply()
		{
			Counter--;
			if (Counter <= 0)
				RPCEvent.Set();
		}

		void GetPublicSummonerData(AllPublicSummonerDataDTO publicSummonerData)
		{
			PublicSummonerData = publicSummonerData;
			ProcessReply();
		}

		void GetLifeTimeStatistics(PlayerLifeTimeStats lifeTimeStatistics)
		{
			LifeTimeStatistics = lifeTimeStatistics;
			ProcessReply();
		}

		void GetAggregatedStatistics(AggregatedStats aggregatedStatistics, int season)
		{
			AggregatedStatistics[season] = aggregatedStatistics;
			ProcessReply();
		}

		void GetRecentGameData(RecentGames recentGameData)
		{
			RecentGameData = recentGameData;
			ProcessReply();
		}

		void Error(Fault fault)
		{
			ErrorOccurred = true;
			ProcessReply();
		}
	}
}
