using System;
using System.Collections.Generic;
using System.Data.Common;

using Blighttp;

namespace RiotGear
{
	public partial class WebService
	{
		DatabaseCommand Command(string query, DbConnection connection, params object[] arguments)
		{
			return new DatabaseCommand(query, connection, WebServiceProfiler, arguments);
		}

		SummonerStatistics GetSummonerStatistics(Summoner summoner, DbConnection connection)
		{
			List<SummonerRating> ratings = GetSummonerRatings(summoner, connection);
			List<List<SummonerRankedStatistics>> rankedStatistics = new List<List<SummonerRankedStatistics>>();
			for (int i = 0; i <= ProgramConfiguration.RankedSeason; i++)
				rankedStatistics.Add(GetSummonerRankedStatistics(summoner, i, connection));
			List<AggregatedChampionStatistics> twistedTreelineStatistics = LoadAggregatedChampionStatistics(summoner, MapType.TwistedTreeline, GameModeType.Normal, connection);
			List<AggregatedChampionStatistics> summonersRiftStatistics = LoadAggregatedChampionStatistics(summoner, MapType.SummonersRift, GameModeType.Normal, connection);
			List<AggregatedChampionStatistics> dominionStatistics = LoadAggregatedChampionStatistics(summoner, MapType.Dominion, GameModeType.Normal, connection);
			SummonerStatistics statistics = new SummonerStatistics(ratings, rankedStatistics, twistedTreelineStatistics, summonersRiftStatistics, dominionStatistics);
			return statistics;
		}

		List<SummonerRating> GetSummonerRatings(Summoner summoner, DbConnection connection)
		{
			using (var select = Command("select {0} from summoner_rating where summoner_id = :summoner_id", connection, SummonerRating.GetFields()))
			{
				select.Set("summoner_id", summoner.Id);
				using (var reader = select.ExecuteReader())
				{
					List<SummonerRating> output = new List<SummonerRating>();
					while (reader.Read())
					{
						SummonerRating rating = new SummonerRating(reader);
						output.Add(rating);
					}
					return output;
				}
			}
		}

		List<SummonerRankedStatistics> GetSummonerRankedStatistics(Summoner summoner, int season, DbConnection connection)
		{
			using (var select = Command("select {0} from summoner_ranked_statistics where summoner_id = :summoner_id and season = :season", connection, SummonerRankedStatistics.GetFields()))
			{
				select.Set("summoner_id", summoner.Id);
				select.Set("season", season);
				using (var reader = select.ExecuteReader())
				{
					List<SummonerRankedStatistics> output = new List<SummonerRankedStatistics>();
					while (reader.Read())
					{
						SummonerRankedStatistics statistics = new SummonerRankedStatistics(reader);
						output.Add(statistics);
					}
					return output;
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
				string createViewQuery = "create temporary view {0} as select game.map, game.game_mode, game.blue_team_id, game.purple_team_id, game.blue_team_won, player.team_id, player.summoner_id, player.champion_id, player.kills, player.deaths, player.assists, player.gold, player.minion_kills from game, player where game.blue_team_id = player.team_id or game.purple_team_id = player.team_id";
				using (var createView = Command(createViewQuery, connection, viewName))
				{
					createView.Execute();
					string commonWhereClause = string.Format("{0}.summoner_id = :summoner_id and {0}.map = :map and {0}.game_mode = :game_mode", viewName);
					string selectQuery =
						"select statistics.champion_id, coalesce(champion_wins.wins, 0) as wins, coalesce(champion_losses.losses, 0) as losses, statistics.kills, statistics.deaths, statistics.assists, statistics.gold, statistics.minion_kills from " +
						"(select {0}.champion_id, sum({0}.kills) as kills, sum({0}.deaths) as deaths, sum({0}.assists) as assists, sum({0}.gold) as gold, sum({0}.minion_kills) as minion_kills from {0} where {1} group by {0}.champion_id) " +
						"as statistics " +
						"left outer join " +
						"(select champion_id, count(*) as wins from {0} where {1} and ((blue_team_won = 1 and blue_team_id = team_id) or (blue_team_won = 0 and purple_team_id = team_id)) group by champion_id) " +
						"as champion_wins " +
						"on statistics.champion_id = champion_wins.champion_id " +
						"left outer join " +
						"(select champion_id, count(*) as losses from {0} where {1} and ((blue_team_won = 0 and blue_team_id = team_id) or (blue_team_won = 1 and purple_team_id = team_id)) group by champion_id) " +
						"as champion_losses " +
						"on statistics.champion_id = champion_losses.champion_id";
					using (var select = Command(selectQuery, connection, viewName, commonWhereClause))
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
								output.Add(statistics);
							}
							using (var dropView = Command("drop view {0}", connection, viewName))
								dropView.Execute();
							return output;
						}
					}
				}
			}
			finally
			{
				ReleaseViewName(viewName);
			}
		}

		List<ExtendedPlayer> GetSummonerGames(Summoner summoner, DbConnection connection)
		{
			List<ExtendedPlayer> output = new List<ExtendedPlayer>();
			using (var select = Command("select {0} from game, player where game.id = player.game_id and player.summoner_id = :summoner_id order by game.time desc", connection, ExtendedPlayer.GetFields()))
			{
				select.Set("summoner_id", summoner.Id);
				using (var reader = select.ExecuteReader())
				{
					while (reader.Read())
					{
						ExtendedPlayer player = new ExtendedPlayer(reader);
						output.Add(player);
					}
				}
				return output;
			}
		}

		OperationResult SetSummonerAutomaticUpdates(Summoner summoner, bool updateAutomatically)
		{
			using (var connection = GetConnection())
			{
				using (var update = Command("update summoner set update_automatically = :update_automatically where region = :region and account_id = :account_id", connection))
				{
					update.Set("update_automatically", updateAutomatically);
					update.Set("region", summoner.Region);
					update.Set("account_id", summoner.AccountId);
					int rowsAffected = update.Execute();
					if (rowsAffected > 0)
					{
						summoner.UpdateAutomatically = updateAutomatically;
						return OperationResult.Success;
					}
					else
						return OperationResult.NotFound;
				}
			}
		}

		List<RunePage> GetRunePages(Summoner summoner)
		{
			List<RunePage> output = new List<RunePage>();
			using (var connection = GetConnection())
			{
				using (var pageSelect = Command("select {0} from rune_page where summoner_id = :summoner_id", connection, RunePage.GetFields()))
				{
					pageSelect.Set("summoner_id", summoner.Id);

					using (var pageReader = pageSelect.ExecuteReader())
					{
						while (pageReader.Read())
						{
							RunePage page = new RunePage(pageReader);

							using (var slotSelect = Command("select {0} from rune_slot where rune_page_id = :rune_page_id", connection, RuneSlot.GetFields()))
							{
								slotSelect.Set("rune_page_id", page.Id);

								using (var slotReader = slotSelect.ExecuteReader())
								{
									while (slotReader.Read())
									{
										RuneSlot slot = new RuneSlot(slotReader);
										page.Slots.Add(slot);
									}
								}
							}

							page.Slots.Sort((x, y) => x.Slot.CompareTo(y.Slot));

							output.Add(page);
						}
					}
				}
			}

			return output;
		}
	}
}
