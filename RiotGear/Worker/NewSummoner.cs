using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using LibOfLegends;
using Nil;

using com.riotgames.platform.summoner;

namespace RiotGear
{
	public partial class Worker
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

			"has_been_updated",

			"update_automatically",

			"time_created",

			"time_updated",
		};

		void InsertNewSummoner(Summoner summoner, DbConnection connection)
		{
			//We are dealing with a new summoner
			string query = string.Format("insert into summoner ({0}) values ({1})", GetGroupString(NewSummonerFields), GetPlaceholderString(NewSummonerFields));
			using (var newSummoner = Command(query, connection))
			{
				newSummoner.SetFieldNames(NewSummonerFields);

				newSummoner.Set(Profile.Identifier);

				newSummoner.Set(summoner.AccountId);
				newSummoner.Set(summoner.SummonerId);

				newSummoner.Set(summoner.SummonerName);
				newSummoner.Set(summoner.InternalName);

				newSummoner.Set(summoner.SummonerLevel);
				newSummoner.Set(summoner.ProfileIcon);

				newSummoner.Set(summoner.HasBeenUpdated);

				newSummoner.Set(summoner.UpdateAutomatically);

				long timestamp = Time.UnixTime();

				newSummoner.Set(timestamp);
				newSummoner.Set(timestamp);

				newSummoner.Execute();

				summoner.Id = GetInsertId(connection);
			}
		}
	}
}
