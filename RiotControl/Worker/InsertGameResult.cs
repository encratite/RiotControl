using System.Collections.Generic;

using LibOfLegends;

using com.riotgames.platform.statistics;

namespace RiotControl
{
	partial class Worker
	{
		void InsertGameResult(SummonerDescription summoner, int gameId, int teamId, PlayerGameStats game, GameResult gameResult)
		{
			List<string> fields = new List<string>()
			{
				"game_id",
				"team_id",
				"summoner_id",

				"ping",
				"time_spent_in_queue",

				"premade_size",

				"k_coefficient",
				"probability_of_winning",

				"rating",
				"rating_change",
				"adjusted_rating",
				"team_rating",

				"experience_earned",
				"boosted_experience_earned",

				"ip_earned",
				"boosted_ip_earned",

				"summoner_level",

				"summoner_spell1",
				"summoner_spell2",

				"champion_id",

				"skin_name",
				"skin_index",

				"champion_level",

				//Items are stored as an SQL array
				"items",

				"kills",
				"deaths",
				"assists",

				"minion_kills",

				"gold",

				"damage_dealt",
				"physical_damage_dealt",
				"magical_damage_dealt",

				"damage_taken",
				"physical_damage_taken",
				"magical_damage_taken",

				"total_healing_done",

				"time_spent_dead",

				"largest_multikill",
				"largest_killing_spree",
				"largest_critical_strike",

				//Summoner's Rift/Twisted Treeline

				"neutral_minions_killed",

				"turrets_destroyed",
				"inhibitors_destroyed",

				//Dominion

				"nodes_neutralised",
				"node_neutralisation_assists",
				"nodes_captured",

				"victory_points",
				"objectives",

				"total_score",
				"objective_score",
				"combat_score",

				"rank",
			};

			string queryFields = GetGroupString(fields);
			string queryValues = GetPlaceholderString(fields);
			DatabaseCommand insert = Command("insert into player ({0}) values ({1})", queryFields, queryValues);
			insert.SetFieldNames(fields);

			insert.Set(gameId);
			insert.Set(teamId);
			insert.Set(summoner.Id);

			insert.Set(game.userServerPing);
			insert.Set(game.timeInQueue);

			insert.Set(game.premadeSize);

			insert.Set(game.KCoefficient);
			insert.Set(game.predictedWinPct);

			insert.Set(game.rating);
			insert.Set(game.eloChange);
			insert.Set(game.adjustedRating);
			insert.Set(game.teamRating);

			insert.Set(game.experienceEarned);
			insert.Set(game.boostXpEarned);

			insert.Set(game.ipEarned);
			insert.Set(game.boostIpEarned);

			insert.Set(game.level);

			insert.Set(game.spell1);
			insert.Set(game.spell2);

			insert.Set(game.championId);

			insert.Set(game.skinName);
			insert.Set(game.skinIndex);

			insert.Set(gameResult.Level);

			//Items are an array of integers and require special treatment
			string itemString = string.Format("'{{{0}}}'", string.Join(", ", gameResult.Items));
			insert.Set(itemString);

			insert.Set(gameResult.Kills);
			insert.Set(gameResult.Deaths);
			insert.Set(gameResult.Assists);

			insert.Set(gameResult.MinionsKilled);

			insert.Set(gameResult.GoldEarned);

			insert.Set(gameResult.TotalDamageDealt);
			insert.Set(gameResult.PhysicalDamageDealt);
			insert.Set(gameResult.MagicalDamageDealt);

			insert.Set(gameResult.TotalDamageTaken);
			insert.Set(gameResult.PhysicalDamageTaken);
			insert.Set(gameResult.MagicalDamageTaken);

			insert.Set(gameResult.TotalHealingDone);

			insert.Set(gameResult.TimeSpentDead);

			insert.Set(gameResult.LargestMultiKill);
			insert.Set(gameResult.LargestKillingSpree);
			insert.Set(gameResult.LargestCriticalStrike);

			//Summoner's Rift/Twisted Treeline

			insert.Set(gameResult.NeutralMinionsKilled);

			insert.Set(gameResult.TurretsDestroyed);
			insert.Set(gameResult.InhibitorsDestroyed);

			//Dominion

			insert.Set(gameResult.NodesNeutralised);
			insert.Set(gameResult.NodeNeutralisationAssists);
			insert.Set(gameResult.NodesCaptured);

			insert.Set(gameResult.VictoryPoints);
			insert.Set(gameResult.Objectives);

			insert.Set(gameResult.TotalScore);
			insert.Set(gameResult.ObjectiveScore);
			insert.Set(gameResult.CombatScore);

			insert.Set(gameResult.Rank);

			insert.Execute();
		}
	}
}
