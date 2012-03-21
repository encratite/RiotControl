using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Nil;

using com.riotgames.platform.statistics;

namespace RiotControl
{
	partial class Worker
	{
		void SetSummaryParameters(DatabaseCommand command, MapType map, GameModeType gameMode, SummonerDescription summoner, PlayerStatSummary summary, bool forceNullRating)
		{
			if (forceNullRating)
			{
				command.Set("current_rating", DbType.Int32, null);
				command.Set("top_rating", DbType.Int32, null);
			}
			else
			{
				//Zero rating means that the Elo is below 1200 and is not revealed by the server
				if (summary.rating == 0)
					command.Set("current_rating", DbType.Int32, null);
				else
					command.Set("current_rating", summary.rating);
				command.Set("top_rating", summary.maxRating);
			}

			command.Set("summoner_id", summoner.Id);
			command.Set("map", (int)map);
			command.Set("game_mode", (int)gameMode);

			command.Set("wins", summary.wins);
			command.Set("losses", summary.losses);
			command.Set("leaves", summary.leaves);
		}
		void ProcessSummary(MapType map, GameModeType gameMode, string target, SummonerDescription summoner, List<PlayerStatSummary> summaries, bool forceNullRating = false)
		{
			foreach (var summary in summaries)
			{
				if (summary.playerStatSummaryType != target)
					continue;
				using (var update = Command("update summoner_rating set wins = :wins, losses = :losses, leaves = :leaves, current_rating = :current_rating, top_rating = :top_rating where summoner_id = :summoner_id and map = :map and game_mode = :game_mode"))
				{
					SetSummaryParameters(update, map, gameMode, summoner, summary, forceNullRating);

					int rowsAffected = update.Execute();
					if (rowsAffected == 0)
					{
						//We're dealing with a new summoner rating entry, insert it
						using (var insert = Command("insert into summoner_rating (summoner_id, map, game_mode, wins, losses, leaves, current_rating, top_rating) values (:summoner_id, :map, :game_mode, :wins, :losses, :leaves, :current_rating, :top_rating)"))
						{
							SetSummaryParameters(insert, map, gameMode, summoner, summary, forceNullRating);
							insert.Execute();
							//SummonerMessage(string.Format("New rating for mode {0}", target), summoner);
						}
					}
					else
					{
						//This rating was already in the database and was updated
						//SummonerMessage(string.Format("Updated rating for mode {0}", target), summoner);
					}
					break;
				}
			}
		}

		void UpdateSummonerLastModifiedTimestamp(SummonerDescription summoner)
		{
			using (var timeUpdate = Command("update summoner set time_updated = :time_updated where id = :id"))
			{
				timeUpdate.Set("time_updated", Time.UnixTime());
				timeUpdate.Set("id", summoner.Id);
				timeUpdate.Execute();
			}
		}

		void UpdateSummonerRatings(SummonerDescription summoner, PlayerLifeTimeStats lifeTimeStatistics)
		{
			List<PlayerStatSummary> summaries = lifeTimeStatistics.playerStatSummaries.playerStatSummarySet;

			ProcessSummary(MapType.SummonersRift, GameModeType.Normal, "Unranked", summoner, summaries, true);
			ProcessSummary(MapType.TwistedTreeline, GameModeType.Premade, "RankedPremade3x3", summoner, summaries);
			ProcessSummary(MapType.SummonersRift, GameModeType.Solo, "RankedSolo5x5", summoner, summaries);
			ProcessSummary(MapType.SummonersRift, GameModeType.Premade, "RankedPremade5x5", summoner, summaries);
			ProcessSummary(MapType.Dominion, GameModeType.Normal, "OdinUnranked", summoner, summaries, true);
		}

		static int CompareGames(PlayerGameStats x, PlayerGameStats y)
		{
			return - x.createDate.CompareTo(y.createDate);
		}

		void UpdateSummonerGames(SummonerDescription summoner, RecentGames recentGameData)
		{
			var recentGames = recentGameData.gameStatistics;
			recentGames.Sort(CompareGames);
			foreach (var game in recentGames)
				UpdateSummonerGame(summoner, game);
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
