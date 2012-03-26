using System.Collections.Generic;

using LibOfLegends;

namespace RiotControl
{
	public class Configuration
	{
		//In seconds
		public int AutomaticUpdateInterval;

		//Path to SQLite database
		public string Database;

		//Path to the index file
		public string Index;

		public WebConfiguration Web;
		public AuthenticationProfile Authentication;

		//Proxy is optional, may be null
		public ProxyProfile Proxy;
		public List<EngineRegionProfile> RegionProfiles;

		public Configuration()
		{
			Authentication = new AuthenticationProfile();
			Proxy = new ProxyProfile();
			RegionProfiles = new List<EngineRegionProfile>();
		}
	}
}
