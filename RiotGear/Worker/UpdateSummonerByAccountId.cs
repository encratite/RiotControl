using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using System.Data.Common;

using LibOfLegends;

using com.riotgames.platform.statistics;
using com.riotgames.platform.summoner;
using com.riotgames.platform.gameclient.domain;

namespace RiotGear
{
	public partial class Worker
	{
		public OperationResult UpdateSummonerByAccountId(int accountId)
		{
			if (!Connected)
				return OperationResult.NotConnected;

			WriteLine("Updating account {0}", accountId);
			ConcurrentRPC concurrentRPC = new ConcurrentRPC(RPC, accountId);
			OperationResult result = concurrentRPC.Run();
			if (result == OperationResult.Success)
			{
				if (concurrentRPC.PublicSummonerData == null)
				{
					//This means that the summoner was not found, even though the other structures are actually non-null
					return OperationResult.NotFound;
				}
				Summoner newSummoner = new Summoner(concurrentRPC.PublicSummonerData, Region);
				Summoner summoner = StatisticsService.GetSummoner(Region, accountId);
				if (summoner == null)
				{
					//The summoner wasn't in the database yet, add them
					using (var connection = Provider.GetConnection())
						InsertNewSummoner(newSummoner, connection);
					summoner = newSummoner;
				}
				else
				{
					//Copy data that might have been changed
					summoner.SummonerName = newSummoner.SummonerName;
					summoner.InternalName = newSummoner.InternalName;

					summoner.SummonerLevel = newSummoner.SummonerLevel;
					summoner.ProfileIcon = newSummoner.ProfileIcon;
				}
				//Perform a full update
				using (var connection = Provider.GetConnection())
					UpdateSummoner(summoner, concurrentRPC.PublicSummonerData, concurrentRPC.AggregatedStatistics, concurrentRPC.LifeTimeStatistics, concurrentRPC.RecentGameData, connection);
				return OperationResult.Success;
			}
			return result;
		}

		public OperationResult UpdateSummonerByAccountIdSequentially(int accountId)
		{
			const int rpcDelay = 5000;

			if (!Connected)
				return OperationResult.NotConnected;

			WriteLine("Updating account {0}", accountId);

			try
			{
				Thread.Sleep(rpcDelay);
				WriteLine("Requesting public summoner data");
				AllPublicSummonerDataDTO publicSummonerData = RPC.GetAllPublicSummonerDataByAccount(accountId);
				if (publicSummonerData == null)
				{
					// Summoner could not be found
					return OperationResult.NotFound;
				}
				Summoner newSummoner = new Summoner(publicSummonerData, Region);
				Summoner summoner = StatisticsService.GetSummoner(Region, accountId);
				if (summoner == null)
				{
					//The summoner wasn't in the database yet, add them
					using (var connection = Provider.GetConnection())
						InsertNewSummoner(newSummoner, connection);
					summoner = newSummoner;
				}
				else
				{
					//Copy data that might have been changed
					summoner.SummonerName = newSummoner.SummonerName;
					summoner.InternalName = newSummoner.InternalName;

					summoner.SummonerLevel = newSummoner.SummonerLevel;
					summoner.ProfileIcon = newSummoner.ProfileIcon;
				}
				Thread.Sleep(rpcDelay);
				WriteLine("Requesting lifetime statistics");
				PlayerLifeTimeStats lifeTimeStatistics = RPC.RetrievePlayerStatsByAccountID(accountId, "CURRENT");
				if (lifeTimeStatistics == null)
				{
					WriteLine("Failed to retrieve lifetime statistics of user {0}", summoner.SummonerName);
					return OperationResult.NotFound;
				}
				Thread.Sleep(rpcDelay);
				WriteLine("Requesting aggregated statistics");
				AggregatedStats aggregatedStatistics = RPC.GetAggregatedStats(accountId, "CLASSIC", "CURRENT");
				if (aggregatedStatistics == null)
				{
					WriteLine("Failed to retrieve aggregated statistics of user {0}", summoner.SummonerName);
					return OperationResult.NotFound;
				}
				Thread.Sleep(rpcDelay);
				WriteLine("Requesting recent games");
				RecentGames recentGames = RPC.GetRecentGames(accountId);
				if (recentGames == null)
				{
					WriteLine("Failed to retrieve the recent games of user {0}", summoner.SummonerName);
					return OperationResult.NotFound;
				}
				//Perform a full update
				using (var connection = Provider.GetConnection())
					UpdateSummoner(summoner, publicSummonerData, aggregatedStatistics, lifeTimeStatistics, recentGames, connection);
				return OperationResult.Success;
			}
			catch (RPCTimeoutException)
			{
				WriteLine("An RPC timeout occurred while updating account {0}", accountId);
				return OperationResult.NotFound;
			}
		}
	}
}
