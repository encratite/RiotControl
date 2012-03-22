using System.Collections.Generic;
using System.Data.Common;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		DatabaseCommand Command(string query, DbConnection connection, params object[] arguments)
		{
			return new DatabaseCommand(query, connection, WebServiceProfiler, arguments);
		}

		Summoner LoadSummoner(string regionAbbreviation, int accountId, DbConnection connection)
		{
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			using (var select = Command("select {0} from summoner where region = :region and account_id = :account_id", connection, Summoner.GetFields()))
			{
				select.Set("region", worker.WorkerProfile.Identifier);
				select.Set("account_id", accountId);
				using (var reader = select.ExecuteReader())
				{
					if (reader.Read())
					{
						Summoner summoner = new Summoner(reader);
						LoadSummonerRating(summoner, connection);
						LoadSummonerRankedStatistics(summoner, connection);
						summoner.NormalStatistics = LoadAggregatedChampionStatistics(summoner, MapType.SummonersRift, GameModeType.Normal, connection);
						summoner.DominionStatistics = LoadAggregatedChampionStatistics(summoner, MapType.Dominion, GameModeType.Normal, connection);
						return summoner;
					}
					else
						return null;
				}
			}
		}

		int CompareRatings(SummonerRating x, SummonerRating y)
		{
			int output = x.Map.CompareTo(y.Map);
			if (output == 0)
				return x.GameMode.CompareTo(y.GameMode);
			else
				return output;
		}

		void LoadSummonerRating(Summoner summoner, DbConnection connection)
		{
			using (var select = Command("select {0} from summoner_rating where summoner_id = :summoner_id", connection, SummonerRating.GetFields()))
			{
				select.Set("summoner_id", summoner.Id);
				using (var reader = select.ExecuteReader())
				{
					while (reader.Read())
					{
						SummonerRating rating = new SummonerRating(reader);
						summoner.Ratings.Add(rating);
					}
					summoner.Ratings.Sort(CompareRatings);
				}
			}
		}

		void LoadSummonerRankedStatistics(Summoner summoner, DbConnection connection)
		{
			using (var select = Command("select {0} from summoner_ranked_statistics where summoner_id = :summoner_id", connection, SummonerRankedStatistics.GetFields()))
			{
				select.Set("summoner_id", summoner.Id);
				using (var reader = select.ExecuteReader())
				{
					while (reader.Read())
					{
						SummonerRankedStatistics statistics = new SummonerRankedStatistics(reader);
						statistics.ChampionName = GetChampionName(statistics.ChampionId);
						summoner.RankedStatistics.Add(statistics);
					}
					summoner.RankedStatistics.Sort();
				}
			}
		}

		string GetViewName()
		{
			lock (Views)
			{
				while (true)
				{
					string name = string.Format("view_{0}", PRNG.Next());
					if (!Views.Contains(name))
					{
						Views.Add(name);
						return name;
					}
				}
			}
		}

		void ReleaseViewName(string name)
		{
			lock (Views)
				Views.Remove(name);
		}

		List<AggregatedChampionStatistics> LoadAggregatedChampionStatistics(Summoner summoner, MapType map, GameModeType gameMode, DbConnection connection)
		{
			string viewName = GetViewName();
			try
			{
				//Create a temporary view with a dynamically generated name to emulate the former CTE
				string createViewQuery = "create temporary view {0} as select player.champion_id, player.won, player.kills, player.deaths, player.assists, player.gold, player.minion_kills from game, player where game.map = :map and game.game_mode = :game_mode and (game.blue_team_id = player.team_id or game.purple_team_id = player.team_id) and player.summoner_id = :summoner_id";
				using (var createView = Command(createViewQuery, connection, viewName))
				{
					createView.Set("map", map);
					createView.Set("game_mode", gameMode);
					createView.Set("summoner_id", summoner.Id);
					createView.Execute();
					string selectQuery =
						"select statistics.champion_id, coalesce(champion_wins.wins, 0) as wins, coalesce(champion_losses.losses, 0) as losses, statistics.kills, statistics.deaths, statistics.assists, statistics.gold, statistics.minion_kills from " +
						"(select {0}.champion_id, sum({0}.kills) as kills, sum({0}.deaths) as deaths, sum({0}.assists) as assists, sum({0}.gold) as gold, sum({0}.minion_kills) as minion_kills from {0} group by {0}.champion_id) " +
						"as statistics " +
						"left outer join " +
						"(select champion_id, count(*) as wins from {0} where won = true group by champion_id) " +
						"as champion_wins " +
						"on statistics.champion_id = champion_wins.champion_id " +
						"left outer join " +
						"(select champion_id, count(*) as losses from {0} where won = false group by champion_id) " +
						"as champion_losses " +
						"on statistics.champion_id = champion_losses.champion_id;";
					using (var select = Command(selectQuery, connection, viewName))
					{
						select.Set("map", map);
						select.Set("game_mode", gameMode);
						select.Set("summoner_id", summoner.Id);
						using (var reader = select.ExecuteReader())
						{
							List<AggregatedChampionStatistics> output = new List<AggregatedChampionStatistics>();
							while (reader.Read())
							{
								AggregatedChampionStatistics statistics = new AggregatedChampionStatistics(reader);
								statistics.ChampionName = GetChampionName(statistics.ChampionId);
								output.Add(statistics);
							}
							output.Sort();
							return output;
						}
					}
				}
				using (var dropView = Command("drop view {0}", connection, viewName))
					dropView.Execute();
			}
			finally
			{
				ReleaseViewName(viewName);
			}
		}

		void LoadChampionNames()
		{
			ChampionNames = new Dictionary<int, string>();
			using (var database = DatabaseProvider.GetConnection())
			{
				using (var select = Command("select champion_id, champion_name from champion_name", database))
				{
					using (var reader = select.ExecuteReader())
					{
						while (reader.Read())
						{
							int championId = reader.Integer();
							string championName = reader.String();
							ChampionNames[championId] = championName;
						}
					}
				}
			}
		}

		void LoadItemInformation()
		{
			Items = new Dictionary<int, ItemInformation>();
			using (var connection = DatabaseProvider.GetConnection())
			{
				using (var select = Command("select item_id, item_name, description from item_information", connection))
				{
					using (var reader = select.ExecuteReader())
					{
						while (reader.Read())
						{
							int id = reader.Integer();
							string name = reader.String();
							string description = reader.String();
							ItemInformation item = new ItemInformation(id, name, description);
							Items[id] = item;
						}
					}
				}
			}
		}
	}
}
