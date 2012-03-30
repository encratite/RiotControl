using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using System.Data.Common;

using LibOfLegends;

using com.riotgames.platform.statistics;
using com.riotgames.platform.summoner;
using com.riotgames.platform.gameclient.domain;

namespace RiotControl
{
	partial class Worker
	{
		public OperationResult UpdateSummonerByAccountId(int accountId)
		{
			if (!Connected)
				return OperationResult.NotConnected;

			try
			{
				//It is sub-optimal to have this blocking RPC before performing multiple concurrent non-blocking RPCs
				//The only advantage is that it avoids hammering the server with several invalid requests at once
				AllPublicSummonerDataDTO publicSummonerData = RPC.GetAllPublicSummonerDataByAccount(accountId);
				if (publicSummonerData != null)
				{
					Summoner newSummoner = new Summoner(publicSummonerData, Region);
					Summoner summoner = Master.GetSummoner(Region, publicSummonerData.summoner.acctId);
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
						UpdateSummoner(summoner, connection);
					return OperationResult.Success;
				}
				else
				{
					//The summoner could not be found on the server
					return OperationResult.NotFound;
				}
			}
			catch (RPCTimeoutException)
			{
				return OperationResult.Timeout;
			}
		}
	}
}
