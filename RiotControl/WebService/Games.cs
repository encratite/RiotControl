using System.Collections.Generic;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		string GetSummonerGamesTable(List<GameTeamPlayer> games)
		{
			string[] titles =
			{
				"Champion",
				"Map",
				"Mode",
				"Date",
				"K",
				"D",
				"A",
				"MK",
				"NK",
				"Gold",
				"Rating",
				"Adjusted rating",
				"Team rating",
				"Items",
				"Premade",
				"Ping",
				"Time in queue",
			};
			string firstRow = "";
			foreach (var title in titles)
				firstRow += Markup.TableHead(title);
			string rows = Markup.TableRow(firstRow);
			foreach (var game in games)
			{
				string championName = GetChampionName(game.ChampionId);
				string championDescription = Markup.Image(GetImage(string.Format("Champion/Small/{0}.png", Markup.UriEncode(championName))), championName) + championName;
				string items = "\n";
				foreach (var itemId in game.Items)
				{
					if (itemId == 0)
						items += Markup.Image(GetImage("Item/Small/Blank.png"), "Unused");
					else
					{
						ItemInformation item = GetItemInformation(itemId);
						items += Markup.Image(GetImage(string.Format("Item/Small/{0}.png", itemId)), item.Name);
					}
				}
				string noValue = "-";
				string ratingDescription = noValue;
				if (game.Rating.HasValue && game.RatingChange.HasValue && game.Rating > 0)
					ratingDescription = string.Format("{0} ({1})", game.Rating + game.RatingChange, SignumString(game.RatingChange.Value));
				string adjustedRating = noValue;
				if (game.AdjustedRating.HasValue && game.AdjustedRating.Value > 0)
					adjustedRating = game.AdjustedRating.ToString();
				string teamRating = noValue;
				if(game.Rating.HasValue && game.TeamRating.HasValue && game.Rating > 0)
					teamRating = string.Format("{0} ({1})", game.TeamRating, SignumString(game.Rating.Value - game.TeamRating.Value));
				string[] fields1 =
				{
					championDescription,
					game.Map.GetString(),
					game.GameMode.GetString(),
					game.GameTime.ToString(),
					game.Kills.ToString(),
					game.Deaths.ToString(),
					game.Assists.ToString(),
					game.MinionKills.ToString(),
					game.NeutralMinionsKilled.HasValue ? game.NeutralMinionsKilled.ToString() : noValue,
					game.Gold.ToString(),
					ratingDescription,
					adjustedRating,
					teamRating,
				};
				string row = "";
				foreach (var field in fields1)
					row += Markup.TableCell(field);
				row += Markup.ContentTag("td", items, new Dictionary<string, string>() {{"class", "items"}}, true);
				string[] fields2 =
				{
					game.PremadeSize <= 1 ? "No" : string.Format("Yes, {0}", game.PremadeSize),
					string.Format("{0} ms", game.Ping),
					string.Format("{0} s", game.TimeSpentInQueue),
				};
				foreach (var field in fields2)
					row += Markup.TableCell(field);
				rows += Markup.TableRow(row, style: game.Won ? "win" : "loss");
			}
			string table = Markup.Table(rows, style: "statistics", id: "summonerGames");
			return table;
		}
	}
}
