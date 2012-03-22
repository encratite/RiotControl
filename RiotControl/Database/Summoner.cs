using System;
using System.Collections.Generic;

namespace RiotControl
{
	public class Summoner
	{
		public int Id;

		public RegionType Region;

		public int AccountId;
		public int SummonerId;

		public string SummonerName;
		public string InternalName;

		public int SummonerLevel;
		public int ProfileIcon;

		public bool UpdateAutomatically;

		public DateTime TimeCreated;
		public DateTime TimeUpdated;

		//Not part of the summoner table

		public List<SummonerRating> Ratings;

		public List<SummonerRankedStatistics> RankedStatistics;

		public List<AggregatedChampionStatistics> NormalStatistics;
		public List<AggregatedChampionStatistics> DominionStatistics;

		static string[] Fields =
		{
			"id",

			"region",

			"account_id",
			"summoner_id",

			"summoner_name",
			"internal_name",

			"summoner_level",
			"profile_icon",

			"update_automatically",

			"time_created",

			"time_updated",
		};

		public Summoner(DatabaseReader reader)
		{
			Id = reader.Integer();

			Region = (RegionType)reader.Integer();

			AccountId = reader.Integer();
			SummonerId = reader.Integer();

			SummonerName = reader.String();
			InternalName = reader.String();

			SummonerLevel = reader.Integer();
			ProfileIcon = reader.Integer();

			UpdateAutomatically = reader.Boolean();

			TimeCreated = reader.Time();
			TimeUpdated = reader.Time();

			reader.SanityCheck(Fields);

			Ratings = new List<SummonerRating>();
			RankedStatistics = new List<SummonerRankedStatistics>();
		}

		public static string GetFields()
		{
			return Fields.FieldString();
		}

		public int GetGamesPlayed()
		{
			int gamesPlayed = 0;
			foreach (var rating in Ratings)
				gamesPlayed += rating.Wins + rating.Losses;
			return gamesPlayed;
		}
	}
}
