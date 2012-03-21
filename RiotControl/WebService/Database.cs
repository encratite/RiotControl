using System.Collections.Generic;
using System.Data.Common;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		DatabaseCommand GetCommand(string query, DbConnection connection, params object[] arguments)
		{
			return new DatabaseCommand(query, connection, WebServiceProfiler, arguments);
		}

		Summoner LoadSummoner(string regionAbbreviation, int accountId, DbConnection connection)
		{
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			using (var select = GetCommand("select {0} from summoner where region = :region and account_id = :account_id", connection, Summoner.GetFields()))
			{
				select.Set("region", worker.WorkerProfile.Identifier);
				select.Set("account_id", accountId);
				using (var reader = select.ExecuteReader())
				{
					Summoner summoner = null;
					if (reader.Read())
						summoner = new Summoner(reader);
					if (summoner == null)
						throw new HandlerException("No such summoner");
					return summoner;
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
			using (var select = GetCommand("select {0} from summoner_rating where summoner_id = :summoner_id", connection, SummonerRating.GetFields()))
			{
				select.Set("summoner_id", summoner.Id);
				using (var reader = select.ExecuteReader())
				{
					while (reader.Read())
					{
						SummonerRating rating = new SummonerRating(reader);
						summoner.Ratings.Add(rating);
						Dictionary<GameModeType, SummonerRating> dictionary;
						if (!summoner.RatingDictionary.TryGetValue(rating.Map, out dictionary))
						{
							dictionary = new Dictionary<GameModeType, SummonerRating>();
							summoner.RatingDictionary[rating.Map] = dictionary;
						}
						dictionary[rating.GameMode] = rating;
					}
					summoner.Ratings.Sort(CompareRatings);
				}
			}
		}

		void LoadSummonerRankedStatistics(Summoner summoner, DbConnection connection)
		{
			using (var select = GetCommand("select {0} from summoner_ranked_statistics where summoner_id = :summoner_id", connection, SummonerRankedStatistics.GetFields()))
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

		List<AggregatedChampionStatistics> LoadAggregatedChampionStatistics(Summoner summoner, MapType map, GameModeType gameMode, DbConnection connection)
		{
			const string query =
				"with source as " +
				"(select player.champion_id, player.won, player.kills, player.deaths, player.assists, player.gold, player.minion_kills from game, player where game.map = :map and game.game_mode = :game_mode and (game.blue_team_id = player.team_id or game.purple_team_id = player.team_id) and player.summoner_id = :summoner_id) " +
				"select statistics.champion_id, coalesce(champion_wins.wins, 0) as wins, coalesce(champion_losses.losses, 0) as losses, statistics.kills, statistics.deaths, statistics.assists, statistics.gold, statistics.minion_kills from " +
				"(select source.champion_id, sum(source.kills) as kills, sum(source.deaths) as deaths, sum(source.assists) as assists, sum(source.gold) as gold, sum(source.minion_kills) as minion_kills from source group by source.champion_id) " +
				"as statistics " +
				"left outer join " +
				"(select champion_id, count(*) as wins from source where won = true group by champion_id) " +
				"as champion_wins " +
				"on statistics.champion_id = champion_wins.champion_id " +
				"left outer join " +
				"(select champion_id, count(*) as losses from source where won = false group by champion_id) " +
				"as champion_losses " +
				"on statistics.champion_id = champion_losses.champion_id;";
			using (var select = GetCommand(query, connection))
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

		void LoadChampionNames()
		{
			ChampionNames = new Dictionary<int, string>();
			using (var database = DatabaseProvider.GetConnection())
			{
				using (var select = GetCommand("select champion_id, champion_name from champion_name", database))
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
				using (var select = GetCommand("select item_id, item_name, description from item_information", connection))
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
