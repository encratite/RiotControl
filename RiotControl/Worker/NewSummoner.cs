using System.Collections.Generic;
using System.Linq;

using LibOfLegends;

using com.riotgames.platform.summoner;

namespace RiotControl
{
	partial class Worker
	{
		int InsertNewSummoner(int accountId, int summonerId, string name, string internalName, int summonerLevel, int profileIconId)
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
			string valuesString = string.Format(":region, {0}, {1}, {1}", placeholderString, CurrentTimestamp());
			string query = string.Format("insert into summoner ({0}) values ({1})", fieldsString, valuesString);

			DatabaseCommand newSummoner = Command(query);

			newSummoner.Set("region", Profile.Identifier);

			newSummoner.SetFieldNames(coreFields);
			newSummoner.Set(accountId);
			newSummoner.Set(summonerId);
			newSummoner.Set(name);
			newSummoner.Set(internalName);
			newSummoner.Set(summonerLevel);
			newSummoner.Set(profileIconId);
			newSummoner.Set(false);

			newSummoner.Execute();

			int id = GetInsertId();
			UpdateSummoner(new SummonerDescription(name, id, accountId), true);

			return id;
		}
	}
}
