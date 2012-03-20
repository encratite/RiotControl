using System;
using System.Collections.Generic;
using System.Data;

using com.riotgames.platform.statistics;

namespace RiotControl
{
	partial class Worker
	{
		void ProcessSummary(MapType map, GameModeType gameMode, string target, SummonerDescription summoner, List<PlayerStatSummary> summaries, bool forceNullRating = false)
		{
			foreach (var summary in summaries)
			{
				if (summary.playerStatSummaryType != target)
					continue;
				DatabaseCommand update = Command("update summoner_rating set wins = :wins, losses = :losses, leaves = :leaves, current_rating = :current_rating, top_rating = :top_rating where summoner_id = :summoner_id and map = cast(:map as map_type) and game_mode = cast(:game_mode as game_mode_type)");
				if (forceNullRating)
				{
					update.Set("current_rating", DbType.Int32, null);
					update.Set("top_rating", DbType.Int32, null);
				}
				else
				{
					update.Set("current_rating", summary.rating);
					update.Set("top_rating", summary.maxRating);
				}

				update.Set("summoner_id", summoner.Id);
				update.Set("map", (int)map);
				update.Set("game_mode", (int)gameMode);

				update.Set("wins", summary.wins);
				update.Set("losses", summary.losses);
				update.Set("leaves", summary.leaves);

				int rowsAffected = update.Execute();
				if (rowsAffected == 0)
				{
					//We're dealing with a new summoner rating entry, insert it
					DatabaseCommand insert = Command("insert into summoner_rating (summoner_id, map, game_mode, wins, losses, leaves, current_rating, top_rating) values (:summoner_id, cast(:map as map_type), cast(:game_mode as game_mode_type), :wins, :losses, :leaves, :current_rating, :top_rating)");
					insert.CopyParameters(update);
					insert.Execute();
					//SummonerMessage(string.Format("New rating for mode {0}", target), summoner);
				}
				else
				{
					//This rating was already in the database and was updated
					//SummonerMessage(string.Format("Updated rating for mode {0}", target), summoner);
				}
				break;
			}
		}

		void UpdateSummonerLastModifiedTimestamp(SummonerDescription summoner)
		{
			DatabaseCommand timeUpdate = Command(string.Format("update summoner set time_updated = {0} where id = :id", CurrentTimestamp()));
			timeUpdate.Set("id", summoner.Id);
			timeUpdate.Execute();
		}

		void UpdateSummonerRatings(SummonerDescription summoner, PlayerLifeTimeStats lifeTimeStatistics)
		{
			List<PlayerStatSummary> summaries = lifeTimeStatistics.playerStatSummaries.playerStatSummarySet;

			ProcessSummary(MapType.SummonersRift, GameModeType.Normal, "Unranked", summoner, summaries, true);
			ProcessSummary(MapType.TwistedTreeline, GameModeType.Premade, "RankedPremade3x3", summoner, summaries);
			ProcessSummary(MapType.SummonersRift, GameModeType.Solo, "RankedSolo5x5", summoner, summaries);
			ProcessSummary(MapType.SummonersRift, GameModeType.Premade, "RankedPremade5x5", summoner, summaries);
			ProcessSummary(MapType.Dominion, GameModeType.Normal, "OdinUnranked", summoner, summaries);
		}

		static int CompareGames(PlayerGameStats x, PlayerGameStats y)
		{
			return - x.createDate.CompareTo(y.createDate);
		}

		void UpdateNormalRating(SummonerDescription summoner)
		{
			string query =
				"with rating as " +
				"( " +
				"with source as " +
				"(select game_result.game_time, team_player.rating, team_player.rating_change from game_result, team_player where game_result.id = team_player.game_id and game_result.result_map = :map and game_result.game_mode = :game_mode and team_player.summoner_id = :summoner_id) " +
				"select current_rating.current_rating, top_rating.top_rating from " +
				"(select (rating + rating_change) as current_rating from source order by game_time desc limit 1) " +
				"as current_rating, " +
				"(select max(rating + rating_change) as top_rating from source) " +
				"as top_rating " +
				") " +
				"update summoner_rating set current_rating = (select current_rating from rating), top_rating = (select top_rating from rating) where summoner_id = :summoner_id and map = :map and game_mode = :game_mode";
			DatabaseCommand update = Command(query);
			update.Set("map", (int)MapType.SummonersRift);
			update.Set("game_mode", (int)GameModeType.Normal);
			update.Set("summoner_id", summoner.Id);
			update.Execute();
		}

		void UpdateSummonerGames(SummonerDescription summoner, RecentGames recentGameData)
		{
			var recentGames = recentGameData.gameStatistics;
			recentGames.Sort(CompareGames);
			foreach (var game in recentGames)
				UpdateSummonerGame(summoner, game);
			UpdateNormalRating(summoner);
		}

		void UpdateSummoner(SummonerDescription summoner, bool isNewSummoner)
		{
			int accountId = summoner.AccountId;

			lock (ActiveAccountIds)
			{
				//Avoid concurrent updates of the same account, it's asking for trouble and is redundant anyways
				//We might obtain outdated results in one query but that's a minor issue in comparison to corrupted database results
				if (ActiveAccountIds.Contains(accountId))
					return;

				ActiveAccountIds.Add(accountId);
			}

			Profiler profiler = new Profiler(true, string.Format("{0} {1} {2}", Profile.Abbreviation, Profile.Username, summoner.Name));

			SummonerMessage("Updating", summoner);

			profiler.Start("RetrievePlayerStatsByAccountID");
			PlayerLifeTimeStats lifeTimeStatistics = RPC.RetrievePlayerStatsByAccountID(summoner.AccountId, "CURRENT");
			if (lifeTimeStatistics == null)
			{
				SummonerMessage("Unable to retrieve lifetime statistics", summoner);
				return;
			}
			profiler.Stop();

			profiler.Start("GetAggregatedStats");
			AggregatedStats aggregatedStatistics = RPC.GetAggregatedStats(summoner.AccountId, "CLASSIC", "CURRENT");
			if (aggregatedStatistics == null)
			{
				SummonerMessage("Unable to retrieve aggregated statistics", summoner);
				return;
			}
			profiler.Stop();

			profiler.Start("GetRecentGames");
			RecentGames recentGameData = RPC.GetRecentGames(summoner.AccountId);
			if (recentGameData == null)
			{
				SummonerMessage("Unable to retrieve recent games", summoner);
				return;
			}
			profiler.Stop();

			profiler.Start("SQL");
			UpdateSummonerRatings(summoner, lifeTimeStatistics);
			UpdateSummonerRankedStatistics(summoner, aggregatedStatistics);
			UpdateSummonerGames(summoner, recentGameData);

			if (!isNewSummoner)
			{
				//This means that the main summoner entry must be updated
				UpdateSummonerLastModifiedTimestamp(summoner);
			}
			profiler.Stop();

			lock (ActiveAccountIds)
				ActiveAccountIds.Remove(accountId);
		}
	}
}
