using System.Collections.Generic;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		string GetSummonerOverview(string regionName, Summoner summoner)
		{
			string profileIcon = Markup.Image(GetImage(string.Format("Profile/profileIcon{0}.jpg", summoner.ProfileIcon)), string.Format("{0}'s profile icon", summoner.SummonerName), id: "profileIcon");

			var overviewFields1 = new Dictionary<string, string>()
			{
				{"Summoner name", summoner.SummonerName},
				{"Internal name", summoner.InternalName},
				{"Region", regionName},
				{"Summoner level", summoner.SummonerLevel.ToString()},
				{"Non-custom games played", summoner.GetGamesPlayed().ToString()},
			};

			var overviewFields2 = new Dictionary<string, string>()
			{
				{"Account ID", summoner.AccountId.ToString()},
				{"Summoner ID", summoner.SummonerId.ToString()},
				{"First update", summoner.TimeCreated.ToString()},
				{"Last update", summoner.TimeUpdated.ToString()},
				{"Is updated automatically", summoner.UpdateAutomatically ? "Yes" : "No"},
			};

			string overview = Markup.Diverse(profileIcon + GetOverviewTable(overviewFields1) + GetOverviewTable(overviewFields2), id: "summonerHeader");

			return overview;
		}
	}
}
