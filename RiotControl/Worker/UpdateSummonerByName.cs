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
			SQLCommand nameLookup = Command("select id, account_id, summoner_name from summoner where region = cast(:region as region_type) and lower(summoner_name) = lower(:name)");
			nameLookup.SetEnum("region", RegionProfile.RegionEnum);
			nameLookup.Set("name", job.SummonerName);
			NpgsqlDataReader nameReader = nameLookup.ExecuteReader();
			if (nameReader.Read())
			{
				//The summoner already exists in the database
				int id = (int)nameReader[0];
				int accountId = (int)nameReader[1];
				string name = (string)nameReader[2];
				//This is not entirely correct as the name may have changed, but whatever
				job.RealSummonerName = name;
				job.AccountId = accountId;
				UpdateSummoner(new SummonerDescription(name, id, accountId), false);
				job.ProvideResult(name, accountId);
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

				SQLCommand check = Command("select id from summoner where account_id = :account_id");
				check.Set("account_id", publicSummoner.acctId);
				NpgsqlDataReader checkReader = check.ExecuteReader();
				if (checkReader.Read())
				{
					//We are dealing with an existing summoner even though the name lookup failed
					int id = (int)checkReader[0];
					UpdateSummoner(new SummonerDescription(publicSummoner.name, id, publicSummoner.acctId), true);
				}
				else
				{
					//We are dealing with a new summoner
					List<string> coreFields = new List<string>()
					{
						"account_id",
						"summoner_id",
						"summoner_name",
						"internal_name",
						"summoner_level",
						"profile_icon",
						"update_automatically",
					};

					List<string> extendedFields = new List<string>()
					{
						"time_created",
						"time_updated",
					};

					string fieldsString = string.Format("region, {0}", GetGroupString(coreFields.Concat(extendedFields).ToList()));
					string placeholderString = GetPlaceholderString(coreFields);
					string valuesString = string.Format("cast(:region as region_type), {0}, {1}, {1}", placeholderString, CurrentTimestamp());
					string query = string.Format("insert into summoner ({0}) values ({1})", fieldsString, valuesString);

					SQLCommand newSummoner = Command(query);

					newSummoner.SetEnum("region", RegionProfile.RegionEnum);

					newSummoner.SetFieldNames(coreFields);
					newSummoner.Set(publicSummoner.acctId);
					newSummoner.Set(publicSummoner.summonerId);
					newSummoner.Set(publicSummoner.name);
					newSummoner.Set(publicSummoner.internalName);
					newSummoner.Set(publicSummoner.summonerLevel);
					newSummoner.Set(publicSummoner.profileIconId);
					newSummoner.Set(false);

					newSummoner.Execute();

					int id = GetInsertId("summoner");
					UpdateSummoner(new SummonerDescription(publicSummoner.name, id, publicSummoner.acctId), true);
				}
				job.ProvideResult(publicSummoner.name, publicSummoner.acctId);
			}
			nameReader.Close();
		}
	}
}
