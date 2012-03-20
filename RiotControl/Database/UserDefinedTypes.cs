using System;
using System.Collections.Generic;

namespace RiotControl
{
	enum RegionType
	{
		NorthAmerica = 0,
		EuropeWest = 1,
		EuropeNordicEast = 2,
	}

	enum MapType
	{
		TwistedTreeline = 0,
		SummonersRift = 1,
		Dominion = 2,
	}

	enum GameModeType
	{
		Custom = 0,
		Bot = 1,
		Normal = 2,
		Solo = 3,
		Premade = 4,
	}

	static class UserDefinedTypes
	{
		static Dictionary<MapType, string> MapTypeStringDictionary = new Dictionary<MapType, string>()
		{
			{MapType.TwistedTreeline, "Twisted Treeline"},
			{MapType.SummonersRift, "Summoner's Rift"},
			{MapType.Dominion, "Dominion"},
		};

		static Dictionary<GameModeType, string> GameModeTypeStringDictionary = new Dictionary<GameModeType, string>()
		{
			{GameModeType.Custom, "Custom"},
			{GameModeType.Bot, "Co-op vs. AI"},
			{GameModeType.Normal, "Normal"},
			{GameModeType.Solo, "Ranked Solo/Duo"},
			{GameModeType.Premade, "Ranked Teams"},
		};

		public static string GetString(this MapType input)
		{
			return MapTypeStringDictionary[input];
		}

		public static string GetString(this GameModeType input)
		{
			return GameModeTypeStringDictionary[input];
		}
	}
}
