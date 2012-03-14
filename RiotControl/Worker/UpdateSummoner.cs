using System;
using System.Collections.Generic;

using NpgsqlTypes;

using com.riotgames.platform.statistics;

namespace RiotControl
{
	partial class Worker
	{
		void ProcessSummary(string mapEnum, string gameModeEnum, string target, SummonerDescription summoner, List<PlayerStatSummary> summaries, bool forceNullRating = false)
		{
			foreach (var summary in summaries)
			{
				if (summary.playerStatSummaryType != target)
					continue;
				SQLCommand update = Command("update summoner_rating set wins = :wins, losses = :losses, leaves = :leaves, current_rating = :current_rating, top_rating = :top_rating where summoner_id = :summoner_id and rating_map = cast(:rating_map as map_type) and game_mode = cast(:game_mode as game_mode_type)");
				if (forceNullRating)
				{
					update.Set("current_rating", NpgsqlDbType.Integer, null);
					update.Set("top_rating", NpgsqlDbType.Integer, null);
				}
				else
				{
					update.Set("current_rating", summary.rating);
					update.Set("top_rating", summary.maxRating);
				}

				update.Set("summoner_id", summoner.Id);
				update.SetEnum("rating_map", mapEnum);
				update.SetEnum("game_mode", gameModeEnum);

				update.Set("wins", summary.wins);
				update.Set("losses", summary.losses);
				update.Set("leaves", summary.leaves);

				int rowsAffected = update.Execute();
				if (rowsAffected == 0)
				{
					//We're dealing with a new summoner rating entry, insert it
					SQLCommand insert = Command("insert into summoner_rating (summoner_id, rating_map, game_mode, wins, losses, leaves, current_rating, top_rating) values (:summoner_id, cast(:rating_map as map_type), cast(:game_mode as game_mode_type), :wins, :losses, :leaves, :current_rating, :top_rating)");
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
			SQLCommand timeUpdate = Command(string.Format("update summoner set time_updated = {0} where id = :id", CurrentTimestamp()));
			timeUpdate.Set("id", summoner.Id);
			timeUpdate.Execute();
		}

		void UpdateSummonerRatings(SummonerDescription summoner, PlayerLifeTimeStats lifeTimeStatistics)
		{
			List<PlayerStatSummary> summaries = lifeTimeStatistics.playerStatSummaries.playerStatSummarySet;

			ProcessSummary("summoners_rift", "normal", "Unranked", summoner, summaries, true);
			ProcessSummary("twisted_treeline", "premade", "RankedPremade3x3", summoner, summaries);
			ProcessSummary("summoners_rift", "solo", "RankedSolo5x5", summoner, summaries);
			ProcessSummary("summoners_rift", "premade", "RankedPremade5x5", summoner, summaries);
			ProcessSummary("dominion", "normal", "OdinUnranked", summoner, summaries);
		}

		static int CompareGames(PlayerGameStats x, PlayerGameStats y)
		{
			return - x.createDate.CompareTo(y.createDate);
		}

		void UpdateSummonerGames(SummonerDescription summoner, RecentGames recentGameData)
		{
			var recentGames = recentGameData.gameStatistics;
			recentGames.Sort(CompareGames);
			bool foundNormalElo = false;
			int currentNormalElo = 0;
			int topNormalElo = 0;
			foreach (var game in recentGames)
			{
				bool hasNormalElo = false;
				int normalElo = 0;
				UpdateSummonerGame(summoner, game, ref hasNormalElo, ref normalElo);
				if (hasNormalElo)
				{
					if (!foundNormalElo)
					{
						currentNormalElo = normalElo;
						foundNormalElo = true;
					}
					topNormalElo = Math.Max(topNormalElo, normalElo);
				}
			}
			if (foundNormalElo)
			{
				//We discovered a new normal Elo value and must update the database accordingly
				SQLCommand update = Command("update summoner_rating set current_rating = :current_rating, top_rating = greatest(top_rating, :top_rating) where summoner_id = :summoner_id and rating_map = cast('summoners_rift' as map_type) and game_mode = cast('normal' as game_mode_type)");
				update.Set("summoner_id", summoner.Id);
				update.Set("current_rating", currentNormalElo);
				update.Set("top_rating", topNormalElo);
				update.Execute();
			}
		}

		void UpdateSummoner(SummonerDescription summoner, bool isNewSummoner)
		{
			AccountLock accountLock = Master.GetAccountLock(summoner.AccountId);

			lock (accountLock)
			{
				SummonerMessage("Updating", summoner);

				PlayerLifeTimeStats lifeTimeStatistics = RPC.RetrievePlayerStatsByAccountID(summoner.AccountId, "CURRENT");
				if (lifeTimeStatistics == null)
				{
					SummonerMessage("Unable to retrieve lifetime statistics", summoner);
					return;
				}

				AggregatedStats aggregatedStatistics = RPC.GetAggregatedStats(summoner.AccountId, "CLASSIC", "CURRENT");
				if (aggregatedStatistics == null)
				{
					SummonerMessage("Unable to retrieve aggregated statistics", summoner);
					return;
				}

				RecentGames recentGameData = RPC.GetRecentGames(summoner.AccountId);
				if (recentGameData == null)
				{
					SummonerMessage("Unable to retrieve recent games", summoner);
					return;
				}

				UpdateSummonerRatings(summoner, lifeTimeStatistics);
				UpdateSummonerRankedStatistics(summoner, aggregatedStatistics);
				UpdateSummonerGames(summoner, recentGameData);

				if (!isNewSummoner)
				{
					//This means that the main summoner entry must be updated
					UpdateSummonerLastModifiedTimestamp(summoner);
				}

				Master.ReleaseAccountLock(summoner.AccountId, accountLock);
			}
		}
	}
}
