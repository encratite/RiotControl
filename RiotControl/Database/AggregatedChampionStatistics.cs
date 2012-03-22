using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiotControl
{
	public class AggregatedChampionStatistics
	{
		public int ChampionId;

		public int Wins;
		public int Losses;

		public int Kills;
		public int Deaths;
		public int Assists;

		public int Gold;

		public int MinionKills;

		static string[] Fields =
		{
			"champion_id",

			"wins",
			"losses",

			"kills",
			"deaths",
			"assists",

			"gold",
			"minion_kills",
		};

		public AggregatedChampionStatistics(DatabaseReader reader)
		{
			ChampionId = reader.Integer();

			Wins = reader.Integer();
			Losses = reader.Integer();

			Kills = reader.Integer();
			Deaths = reader.Integer();
			Assists = reader.Integer();

			Gold = reader.Integer();

			MinionKills = reader.Integer();

			reader.SanityCheck(Fields);
		}

		public static string GetFields()
		{
			return Fields.FieldString();
		}
	}
}
