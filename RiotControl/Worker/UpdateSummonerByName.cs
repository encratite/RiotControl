using System.Collections.Generic;
using System.Linq;

using Npgsql;

using LibOfLegends;

using com.riotgames.platform.summoner;

namespace RiotControl
{
	partial class Worker
	{
		void UpdateSummonerByName(LookupJob job)
		{
			//Attempt to retrieve an existing account ID to work with in order to avoid looking up the account ID again
			//Perform lower case comparison to account for misspelled versions of the name
			//LoL internally merges these to a mangled "internal name" for lookups anyways
			SQLCommand nameLookup = Command("select id, account_id, summoner_name, summoner_level from summoner where region = cast(:region as region_type) and lower(summoner_name) = lower(:name)");
			nameLookup.SetEnum("region", Profile.RegionEnum);
			nameLookup.Set("name", job.SummonerName);
			using (NpgsqlDataReader nameReader = nameLookup.ExecuteReader())
			{
				if (nameReader.Read())
				{
					//The summoner already exists in the database
					int id = (int)nameReader[0];
					int accountId = (int)nameReader[1];
					string name = (string)nameReader[2];
					int summonerLevel = (int)nameReader[3];
					//This is not entirely correct as the name may have changed, but whatever
					UpdateSummoner(new SummonerDescription(name, id, accountId), false);
					job.ProvideResult(name, accountId, summonerLevel);
				}
				else
				{
					//We might be dealing with a new summoner
					PublicSummoner publicSummoner = RPC.GetSummonerByName(job.SummonerName);
					if (publicSummoner == null)
					{
						WriteLine("No such summoner: {0}", job.SummonerName);
						job.ProvideResult(JobQueryResult.NotFound);
						return;
					}

					SQLCommand check = Command("select id from summoner where account_id = :account_id and region = cast(:region as region_type)");
					check.Set("account_id", publicSummoner.acctId);
					check.SetEnum("region", Profile.RegionEnum);
					using (NpgsqlDataReader checkReader = check.ExecuteReader())
					{
						if (checkReader.Read())
						{
							//We are dealing with an existing summoner even though the name lookup failed
							int id = (int)checkReader[0];
							UpdateSummoner(new SummonerDescription(publicSummoner.name, id, publicSummoner.acctId), true);
						}
						else
							InsertNewSummoner(publicSummoner.acctId, publicSummoner.summonerId, publicSummoner.name, publicSummoner.internalName, publicSummoner.summonerLevel, publicSummoner.profileIconId);

					}
					job.ProvideResult(publicSummoner.name, publicSummoner.acctId, publicSummoner.summonerLevel);
				}
			}
		}
	}
}
