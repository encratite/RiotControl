using System.Collections.Generic;

using LibOfLegends;

namespace RiotGear
{
	public class Configuration
	{
		//This is an URL to a plain text file which contains information about the client version strings used on the different servers
		public string ClientVersionsURL;

		//In seconds
		public int AutomaticUpdateInterval;

		//Reconnection delay in milliseconds to avoid getting banned temporarily for hammering the servers
		public int ReconnectDelay;

		//This project uses System.Data.SQLite on Windows and Mono.Data.Sqlite on Windows to provide access to the SQLite database
		//The provider needs to be configurable because of this
		public string DatabaseProvider;

		//Path to SQLite database
		public string Database;

		//Path to the index file
		public string Index;

		//This indicates the current ranked season
		public int RankedSeason;

		public WebConfiguration Web;
		public AuthenticationProfile Authentication;

		//Proxy is optional, may be null
		public ProxyProfile Proxy;
		public List<EngineRegionProfile> RegionProfiles;

		public Configuration()
		{
			//20 minutes
			AutomaticUpdateInterval = 20 * 60;

			//5 seconds
			ReconnectDelay = 5 * 1000;

			//Default to SQLite instead of Mono
			DatabaseProvider = "System.Data.SQLite";

			Database = "RiotControl.sqlite";

			Index = "Index.html.template";

			RankedSeason = 2;

			Web = new WebConfiguration();
			Authentication = new AuthenticationProfile();

			Proxy = new ProxyProfile();
			RegionProfiles = new List<EngineRegionProfile>();
		}
	}
}
