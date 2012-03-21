using System.Collections.Generic;
using System.Linq;

using LibOfLegends;

using com.riotgames.platform.summoner;

namespace RiotControl
{
	partial class Worker
	{
		//Returns true if the summoner was updated successfully, false otherwise
		public WorkerResult UpdateSummonerByName(string summonerName, ref int accountId)
		{
			if (!Connected)
				return WorkerResult.NotConnected;

			try
			{
				//Attempt to retrieve an existing account ID to work with in order to avoid looking up the account ID again
				//Perform lower case comparison to account for misspelled versions of the name
				//LoL internally merges these to a mangled "internal name" for lookups anyways
				using (var nameLookup = Command("select id, account_id, summoner_name from summoner where region = :region and lower(summoner_name) = lower(:name)"))
				{
					nameLookup.Set("region", Profile.Identifier);
					nameLookup.Set("name", summonerName);
					using (var nameReader = nameLookup.ExecuteReader())
					{
						if (nameReader.Read())
						{
							//The summoner already exists in the database
							int id = nameReader.Integer();
							accountId = nameReader.Integer();
							string name = nameReader.String();
							//This is not entirely correct as the name may have changed, but whatever
							UpdateSummoner(new SummonerDescription(name, id, accountId), false);
							return WorkerResult.Success;
						}
						else
						{
							//We might be dealing with a new summoner
							PublicSummoner publicSummoner = RPC.GetSummonerByName(summonerName);
							if (publicSummoner == null)
							{
								WriteLine("No such summoner: {0}", summonerName);
								return WorkerResult.NotFound;
							}

							using (var check = Command("select id from summoner where account_id = :account_id and region = :region"))
							{
								check.Set("account_id", publicSummoner.acctId);
								check.Set("region", Profile.Identifier);
								using (var checkReader = check.ExecuteReader())
								{
									if (checkReader.Read())
									{
										//We are dealing with an existing summoner even though the name lookup failed
										int id = checkReader.Integer();
										UpdateSummoner(new SummonerDescription(publicSummoner.name, id, publicSummoner.acctId), true);
									}
									else
										InsertNewSummoner(publicSummoner.acctId, publicSummoner.summonerId, publicSummoner.name, publicSummoner.internalName, publicSummoner.summonerLevel, publicSummoner.profileIconId);

								}
								accountId = publicSummoner.acctId;
								return WorkerResult.Success;
							}
						}
					}
				}
			}
			catch (RPCTimeoutException)
			{
				return WorkerResult.Timeout;
			}
		}
	}
}
