using System;
using System.Collections.Generic;

using Nil;

using com.riotgames.platform.summoner;
using com.riotgames.platform.gameclient.domain;

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

		public int TimeCreated;
		public int TimeUpdated;

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

			TimeCreated = reader.Integer();
			TimeUpdated = reader.Integer();

			reader.SanityCheck(Fields);
		}

		public Summoner(PublicSummoner publicSummoner, RegionType summonerRegion)
		{
			Initialise(publicSummoner, summonerRegion);
		}

		public void Initialise(PublicSummoner publicSummoner, RegionType summonerRegion)
		{
			//Id cannot be set yet

			Region = summonerRegion;

			AccountId = publicSummoner.acctId;
			SummonerId = publicSummoner.summonerId;

			SummonerName = publicSummoner.name;
			InternalName = publicSummoner.internalName;

			SummonerLevel = publicSummoner.summonerLevel;
			ProfileIcon = publicSummoner.profileIconId;

			UpdateAutomatically = false;

			int time = (int)Time.UnixTime();

			TimeCreated = time;
			TimeUpdated = time;
		}

		public Summoner(AllPublicSummonerDataDTO publicSummoner, RegionType summonerRegion)
		{
			Initialise(publicSummoner, summonerRegion);
		}

		public void Initialise(AllPublicSummonerDataDTO publicSummoner, RegionType summonerRegion)
		{
			//Id cannot be set yet

			Region = summonerRegion;

			var summoner = publicSummoner.summoner;

			AccountId = summoner.acctId;
			SummonerId = summoner.sumId;

			SummonerName = summoner.name;
			InternalName = summoner.internalName;

			SummonerLevel = publicSummoner.summonerLevel.summonerLevel;
			ProfileIcon = summoner.profileIconId;

			UpdateAutomatically = false;

			int time = (int)Time.UnixTime();

			TimeCreated = time;
			TimeUpdated = time;
		}

		public static string GetFields()
		{
			return Fields.FieldString();
		}
	}
}
