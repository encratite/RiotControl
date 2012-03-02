using System;
using System.Collections.Generic;

namespace RiotControl
{
	public static class Region
	{
		static Dictionary<string, RegionType> RegionDictionary = new Dictionary<string, RegionType>()
		{
			{"north_america", RegionType.NorthAmerica},
			{"europe_west", RegionType.EuropeWest},
			{"europe_nordic_east", RegionType.EuropeNordicEast},
		};

		public static RegionType RegionStringToEnum(this string enumString)
		{
			if (!RegionDictionary.ContainsKey(enumString))
				throw new Exception("Invalid region enumeration string: " + enumString);
			return RegionDictionary[enumString];
		}
	}
}
