using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using LibOfLegends;

using com.riotgames.platform.summoner;

namespace RiotGear
{
	public partial class Worker
	{
		//Returns true if the summoner was updated successfully, false otherwise
		public OperationResult FindSummoner(string summonerName, ref Summoner outputSummoner)
		{
			if (!Connected)
				return OperationResult.NotConnected;

			try
			{
				Summoner summoner = StatisticsService.GetSummoner(Region, summonerName);
				if (summoner != null)
				{
					//The summoner is already in the database, don't update them, just provide the account ID
					//This behaviour is more convenient for the general use case
				}
				else
				{
					//The summoner name is not in the database
					//Retrieve the account ID to see if it's actually a new summoner or just somebody who changed their name
					PublicSummoner publicSummoner = RPC.GetSummonerByName(summonerName);
					if (publicSummoner == null)
					{
						//No such summoner
						return OperationResult.NotFound;
					}

					using (var connection = Provider.GetConnection())
					{
						summoner = StatisticsService.GetSummoner(Region, publicSummoner.acctId);
						if (summoner != null)
						{
							//It's a summoner who was already in the database, just their name changed
							UpdateSummonerFields(summoner, connection);
						}
						else
						{
							//It's a new summoner
							summoner = new Summoner(publicSummoner, Region);
							InsertNewSummoner(summoner, connection);
							StatisticsService.AddSummonerToCache(Region, summoner);
						}
					}
				}
				outputSummoner = summoner;
				return OperationResult.Success;

			}
			catch (RPCTimeoutException)
			{
				return OperationResult.Timeout;
			}
			catch (RPCNotConnectedException)
			{
				return OperationResult.NotConnected;
			}
		}
	}
}
