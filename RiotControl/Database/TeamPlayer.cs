using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Npgsql;

namespace RiotControl.Database
{
	class TeamPlayer
	{
		public int TeamId;
		public int SummonerId;

		public int Ping;
		public int TimeSpentInQueue;

		public int PremadeSize;

		public int KCoefficient;
		public double ProbabilityOfWinning;

		public int? Rating;
		public int? RatingChange;
		public int? AdjustedRating;

		public int ExperienceEarned;
		public int BoostedExperienceEarned;

		public int IPEarned;
		public int BoostedIPEarned;

		public int SummonerLevel;

		public int SummonerSpell1;
		public int SummonerSpell2;

		public int ChampionId;

		//May be null
		public string SkinName;
		public int SkinIndex;

		public int ChampionLevel;

		public int[] Items;

		public int Kills;
		public int Deaths;
		public int Assists;

		public int MinionKills;

		public int Gold;

		public int DamageDealt;
		public int PhysicalDamageDealt;
		public int MagicalDamageDealt;

		public int DamageTaken;
		public int PhysicalDamageTaken;
		public int MagicalDamageTaken;

		public int TotalHealingDone;

		public int TimeSpentDead;

		public int LargestMultiKill;
		public int LargestKillingSpree;
		public int LargestCritcalStrike;

		//Summoner's Rift/Twisted Treeline

		public int? NeutralMinionsKilled;

		public int? TurretsDestroyed;
		public int? InhibitorsDestroyed;

		//Dominion

		public int? NodesNeutralised;
		public int? NodeNeutralisationAssists;
		public int? NodesCaptured;

		public int? VictoryPoints;
		public int? Objectives;

		public int? TotalScore;
		public int? ObjectiveScore;
		public int? CombatScore;

		public int? Rank;

		static string[] Fields =
		{
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

			//Array
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

		public TeamPlayer(NpgsqlDataReader dataReader)
		{
			Reader reader = new Reader(dataReader);

			TeamId = reader.Integer();
			SummonerId = reader.Integer();

			Ping = reader.Integer();
			TimeSpentInQueue = reader.Integer();

			PremadeSize = reader.Integer();

			KCoefficient = reader.Integer();
			ProbabilityOfWinning = reader.Double();

			Rating = reader.MaybeInteger();
			RatingChange = reader.MaybeInteger();
			AdjustedRating = reader.MaybeInteger();

			ExperienceEarned = reader.Integer();
			BoostedExperienceEarned = reader.Integer();

			IPEarned = reader.Integer();
			BoostedIPEarned = reader.Integer();

			SummonerLevel = reader.Integer();

			SummonerSpell1 = reader.Integer();
			SummonerSpell2 = reader.Integer();

			ChampionId = reader.Integer();

			//May be null
			SkinName = reader.String();
			SkinIndex = reader.Integer();

			ChampionLevel = reader.Integer();

			//Not sure about this
			Items = (int[])reader.Get();

			Kills = reader.Integer();
			Deaths = reader.Integer();
			Assists = reader.Integer();

			MinionKills = reader.Integer();

			Gold = reader.Integer();

			DamageDealt = reader.Integer();
			PhysicalDamageDealt = reader.Integer();
			MagicalDamageDealt = reader.Integer();

			DamageTaken = reader.Integer();
			PhysicalDamageTaken = reader.Integer();
			MagicalDamageTaken = reader.Integer();

			TotalHealingDone = reader.Integer();

			TimeSpentDead = reader.Integer();

			LargestMultiKill = reader.Integer();
			LargestKillingSpree = reader.Integer();
			LargestCritcalStrike = reader.Integer();

			//Summoner's Rift/Twisted Treeline

			NeutralMinionsKilled = reader.MaybeInteger();

			TurretsDestroyed = reader.MaybeInteger();
			InhibitorsDestroyed = reader.MaybeInteger();

			//Dominion

			NodesNeutralised = reader.MaybeInteger();
			NodeNeutralisationAssists = reader.MaybeInteger();
			NodesCaptured = reader.MaybeInteger();

			VictoryPoints = reader.MaybeInteger();
			Objectives = reader.MaybeInteger();

			TotalScore = reader.MaybeInteger();
			ObjectiveScore = reader.MaybeInteger();
			CombatScore = reader.MaybeInteger();

			Rank = reader.MaybeInteger();

			reader.SanityCheck(Fields);
		}

		public string GetFields()
		{
			return Fields.FieldString();
		}
	}
}
