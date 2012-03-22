using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiotControl
{
	public class AggregatedChampionStatistics : IComparable
	{
		public int ChampionId;

		public int Wins;
		public int Losses;

		public int Kills;
		public int Deaths;
		public int Assists;

		public int Gold;

		public int MinionKills;

		//Not part of the table

		public string ChampionName;

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

		public int CompareTo(object other)
		{
			if (other.GetType() == typeof(AggregatedChampionStatistics))
			{
				AggregatedChampionStatistics statistics = (AggregatedChampionStatistics)other;
				return ChampionName.CompareTo(statistics.ChampionName);
			}
			else
				throw new ArgumentException("Invalid comparison");
		}
	}
}
