using System;

namespace RiotGear
{
	public class SummonerRankedStatistics
	{
		public int ChampionId;

		public int Wins;
		public int Losses;

		public int Kills;
		public int Deaths;
		public int Assists;

		public int MinionKills;

		public int Gold;

		public int TurretsDestroyed;

		public int DamageDealt;
		public int PhysicalDamageDealt;
		public int MagicalDamageDealt;

		public int DamageTaken;

		public int DoubleKills;
		public int TripleKills;
		public int QuadraKills;
		public int PentaKills;

		public int TimeSpentDead;

		public int MaximumKills;
		public int MaximumDeaths;

		static string[] Fields =
		{
			"champion_id",

			"wins",
			"losses",

			"kills",
			"deaths",
			"assists",

			"minion_kills",

			"gold",

			"turrets_destroyed",

			"damage_dealt",
			"physical_damage_dealt",
			"magical_damage_dealt",

			"damage_taken",

			"double_kills",
			"triple_kills",
			"quadra_kills",
			"penta_kills",

			"time_spent_dead",

			"maximum_kills",
			"maximum_deaths",

		};

		public SummonerRankedStatistics(DatabaseReader reader)
		{
			ChampionId = reader.Integer();

			Wins = reader.Integer();
			Losses = reader.Integer();

			Kills = reader.Integer();
			Deaths = reader.Integer();
			Assists = reader.Integer();

			MinionKills = reader.Integer();

			Gold = reader.Integer();

			TurretsDestroyed = reader.Integer();

			DamageDealt = reader.Integer();
			PhysicalDamageDealt = reader.Integer();
			MagicalDamageDealt = reader.Integer();

			DamageTaken = reader.Integer();

			DoubleKills = reader.Integer();
			TripleKills = reader.Integer();
			QuadraKills = reader.Integer();
			PentaKills = reader.Integer();

			TimeSpentDead = reader.Integer();

			MaximumKills = reader.Integer();
			MaximumDeaths = reader.Integer();

			reader.SanityCheck(Fields);
		}

		public static string GetFields()
		{
			return Fields.FieldString();
		}
	}
}
