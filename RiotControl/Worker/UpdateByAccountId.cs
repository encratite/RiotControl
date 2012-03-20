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
		bool UpdateByAccountId(int accountId)
		{
			using (var nameLookup = Command("select id, summoner_name from summoner where region = :region and account_id = :account_id"))
			{
				nameLookup.Set("region", Profile.Identifier);
				nameLookup.Set("account_id", accountId);
				using (var nameReader = nameLookup.ExecuteReader())
				{
					if (nameReader.Read())
					{
						int id = nameReader.Integer();
						string name = nameReader.String();
						UpdateSummoner(new SummonerDescription(name, id, accountId), false);
						return true;
					}
					else
					{
						//The account isn't in the database yet, add it
						AllPublicSummonerDataDTO publicSummonerData = RPC.GetAllPublicSummonerDataByAccount(accountId);
						if (publicSummonerData != null)
						{
							var summoner = publicSummonerData.summoner;
							int id = InsertNewSummoner(summoner.acctId, summoner.sumId, summoner.name, summoner.internalName, publicSummonerData.summonerLevel.summonerLevel, summoner.profileIconId);
							UpdateSummoner(new SummonerDescription(summoner.name, id, summoner.acctId), false);
							return true;
						}
						else
						{
							//No such summoner
							return false;
						}
					}
				}
			}
		}
	}
}
