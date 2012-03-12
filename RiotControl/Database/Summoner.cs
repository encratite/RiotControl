using System;

using Npgsql;

namespace RiotControl
{
	class Summoner
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

		public Summoner(NpgsqlDataReader dataReader)
		{
			Reader reader = new Reader(dataReader);

			Id = reader.Integer();

			Region = reader.String().ToRegionType();

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
		}

		public string GetFields()
		{
			return Fields.FieldString();
		}
	}
}
