using System.Collections.Generic;

using LibOfLegends;

namespace RiotControl
{
	public class EngineRegionProfile
	{
		public string Description;
		public string Abbreviation;

		//This numeric identifier is the way it is stored in the database
		public int Identifier;

		public RegionProfile Region;

		//Login is null if no account is available for this region
		public Login Login;
	}
}
