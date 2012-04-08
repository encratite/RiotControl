using System;
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
		public UpdateConfiguration Updates;

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

		static void Error(string message, params object[] arguments)
		{
			throw new ConfigurationException(string.Format(message, arguments));
		}

		public static void Check(string name, object target)
		{
			if (target == null)
				Error("Configuration section \"{0}\" was left undefined", name);
		}

		public static void Check(string name, string target)
		{
			if (target == null || target.Length == 0)
				Error("Configuration string \"{0}\" was left undefined", name);
		}	

		//Check for invalid values specified by the user
		public void Check()
		{
			Check("ClientVersionsURL", ClientVersionsURL);
			Check("DatabaseProvider", DatabaseProvider);
			Check("Database", Database);
			Check("Index", Index);

			Check("Web", Web);
			Check("Authentication", Authentication);
			Check("Updates", Updates);

			//Do not check proxy as it may be left undefined

			Check("RegionProfiles", RegionProfiles);

			Web.Check();
			CheckAuthentication();
			Updates.Check();
		}

		void CheckAuthentication()
		{
			//Intentionally left undefined
			//Check("Authentication.ClientVersion", Authentication.ClientVersion);
			Check("Authentication.Domain", Authentication.Domain);
			Check("Authentication.IPAddress", Authentication.IPAddress);
			Check("Authentication.Locale", Authentication.Locale);
		}
	}
}
