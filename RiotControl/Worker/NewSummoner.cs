using System.Collections.Generic;
using System.Linq;

using LibOfLegends;
using Nil;

using com.riotgames.platform.summoner;

namespace RiotControl
{
	partial class Worker
	{
		static string[] NewSummonerFields =
		{
			"region",

			"account_id",
			"summoner_id",

			"summoner_name",
			"internal_name",

			"summoner_level",
			"profile_icon",

			"update_automatically",

			"time_created",

			"time_updated",
		};

		int InsertNewSummoner(int accountId, int summonerId, string name, string internalName, int summonerLevel, int profileIconId)
		{
			//We are dealing with a new summoner
			string query = string.Format("insert into summoner ({0}) values ({1})", GetGroupString(NewSummonerFields), GetPlaceholderString(NewSummonerFields));

			using (var newSummoner = Command(query))
			{
				newSummoner.SetFieldNames(NewSummonerFields);

				newSummoner.Set(Profile.Identifier);

				newSummoner.Set(accountId);
				newSummoner.Set(summonerId);

				newSummoner.Set(name);
				newSummoner.Set(internalName);

				newSummoner.Set(summonerLevel);
				newSummoner.Set(profileIconId);

				newSummoner.Set(false);

				long timestamp = Time.UnixTime();

				newSummoner.Set(timestamp);
				newSummoner.Set(timestamp);

				newSummoner.Execute();

				int id = GetInsertId();
				UpdateSummoner(new SummonerDescription(name, id, accountId), true);

				return id;
			}
		}
	}
}
