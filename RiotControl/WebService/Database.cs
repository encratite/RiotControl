using System.Collections.Generic;

using Npgsql;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		SQLCommand GetCommand(string query, NpgsqlConnection database, params object[] arguments)
		{
			return new SQLCommand(query, database, WebServiceProfiler, arguments);
		}

		Summoner LoadSummoner(string regionName, int accountId, NpgsqlConnection database)
		{
			RegionHandler regionHandler = GetRegionHandler(regionName);
			SQLCommand select = GetCommand("select {0} from summoner where region = cast(:region as region_type) and account_id = :account_id", database, Summoner.GetFields());
			select.SetEnum("region", regionHandler.GetRegionEnum());
			select.Set("account_id", accountId);
			using (NpgsqlDataReader reader = select.ExecuteReader())
			{
				Summoner summoner = null;
				if (reader.Read())
					summoner = new Summoner(reader);
				if (summoner == null)
					throw new HandlerException("No such summoner");
				return summoner;
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

		void LoadSummonerRating(Summoner summoner, NpgsqlConnection database)
		{
			SQLCommand select = GetCommand("select {0} from summoner_rating where summoner_id = :summoner_id", database, SummonerRating.GetFields());
			select.Set("summoner_id", summoner.Id);
			using (NpgsqlDataReader reader = select.ExecuteReader())
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

		void LoadSummonerRankedStatistics(Summoner summoner, NpgsqlConnection database)
		{
			SQLCommand select = GetCommand("select {0} from summoner_ranked_statistics where summoner_id = :summoner_id", database, SummonerRankedStatistics.GetFields());
			select.Set("summoner_id", summoner.Id);
			using (NpgsqlDataReader reader = select.ExecuteReader())
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

		List<AggregatedChampionStatistics> LoadAggregatedChampionStatistics(Summoner summoner, MapType map, GameModeType gameMode, NpgsqlConnection database)
		{
			const string query =
				"with source as " +
				"(select team_player.champion_id, team_player.won, team_player.kills, team_player.deaths, team_player.assists, team_player.gold, team_player.minion_kills from game_result, team_player where game_result.result_map = cast(:result_map as map_type) and game_result.game_mode = cast(:game_mode as game_mode_type) and (game_result.team1_id = team_player.team_id or game_result.team2_id = team_player.team_id) and team_player.summoner_id = :summoner_id) " +
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
			SQLCommand select = GetCommand(query, database);
			select.SetEnum("result_map", map.ToEnumString());
			select.SetEnum("game_mode", gameMode.ToEnumString());
			select.Set("summoner_id", summoner.Id);
			using (NpgsqlDataReader reader = select.ExecuteReader())
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

		void LoadChampionNames()
		{
			ChampionNames = new Dictionary<int, string>();
			using (NpgsqlConnection database = DatabaseProvider.GetConnection())
			{
				SQLCommand select = GetCommand("select champion_id, champion_name from champion_name", database);
				using (NpgsqlDataReader reader = select.ExecuteReader())
				{
					while (reader.Read())
					{
						int championId = (int)reader[0];
						string championName = (string)reader[1];
						ChampionNames[championId] = championName;
					}
				}
			}
		}

		void LoadItemInformation()
		{
			Items = new Dictionary<int, ItemInformation>();
			using (NpgsqlConnection database = DatabaseProvider.GetConnection())
			{
				SQLCommand select = GetCommand("select item_id, item_name, description from item_information", database);
				using (NpgsqlDataReader dataReader = select.ExecuteReader())
				{
					while (dataReader.Read())
					{
						Reader reader = new Reader(dataReader);
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
