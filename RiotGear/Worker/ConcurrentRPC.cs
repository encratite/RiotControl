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
		public AggregatedStats AggregatedStatistics;
		public RecentGames RecentGameData;

		public ConcurrentRPC(RPCService rpc, int accountId)
		{
			RPC = rpc;
			AccountId = accountId;
		}

		public OperationResult Run()
		{
			RPCEvent = new AutoResetEvent(false);
			ErrorOccurred = false;
			Counter = 4;

			RPC.GetAllPublicSummonerDataByAccountAsync(AccountId, new Responder<AllPublicSummonerDataDTO>(GetPublicSummonerData, Error));
			RPC.RetrievePlayerStatsByAccountIDAsync(AccountId, "CURRENT", new Responder<PlayerLifeTimeStats>(GetLifeTimeStatistics, Error));
			RPC.GetAggregatedStatsAsync(AccountId, "CLASSIC", "CURRENT", new Responder<AggregatedStats>(GetAggregatedStatistics, Error));
			RPC.GetRecentGamesAsync(AccountId, new Responder<RecentGames>(GetRecentGameData, Error));

			try
			{
				RPCEvent.WaitOne(RPCTimeout);
				if (ErrorOccurred || PublicSummonerData == null || LifeTimeStatistics == null || AggregatedStatistics == null || RecentGameData == null)
					return OperationResult.NotFound;
				else
					return OperationResult.Success;
			}
			catch (RPCTimeoutException)
			{
				return OperationResult.Timeout;
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

		void GetAggregatedStatistics(AggregatedStats aggregatedStatistics)
		{
			AggregatedStatistics = aggregatedStatistics;
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
