using System.Collections.Generic;
using System.Xml.Serialization;

using LibOfLegends;

namespace RiotGear
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

		//This is set dynamically and not part of the configuration
		[XmlIgnore]
		public string ClientVersion;
	}
}
