using System;
using System.Collections.Generic;

using LibOfLegends;

namespace RiotGear
{
	public class Configuration
	{
		//This is an URL to a plain text file which contains information about the client version strings used on the different servers
		public string ClientVersionsURL;

		//The time a worker waits after completing a full round of updates for the region, in seconds
		//If the value chosen is too low, the server will get hammered too much for no good reason
		public int AutomaticUpdateInterval;

		//The time a worker waits after updating a single summoner, in seconds
		//If the value chosen is too low, the server will drop the client and the updates won't be completed
		public int AutomaticUpdateDelayPerSummoner;

		//Reconnection delay in milliseconds to avoid getting banned temporarily for hammering the servers
		public int ReconnectDelay;

		//This project uses System.Data.SQLite on Windows and Mono.Data.Sqlite on Windows to provide access to the SQLite database
		//The provider needs to be configurable because of this
		public string DatabaseProvider;

		//Path to SQLite database
		public string Database;

		//Path to the index file
		public string Index;

		public WebConfiguration Web;
		public AuthenticationProfile Authentication;
		public UpdateConfiguration Updates;

		//Proxy is optional, may be null
		public ProxyProfile Proxy;

		public List<PrivilegeClass> Privileges;

		public List<EngineRegionProfile> RegionProfiles;

		public Configuration()
		{
			//20 minutes
			AutomaticUpdateInterval = 20 * 60;

			//2 seconds
			AutomaticUpdateDelayPerSummoner = 3 * 1000;

			//5 seconds
			ReconnectDelay = 5 * 1000;

			//Default to SQLite instead of Mono
			DatabaseProvider = "System.Data.SQLite";

			Database = "RiotControl.sqlite";

			Index = "Index.html.template";

			Web = new WebConfiguration();
			Authentication = new AuthenticationProfile();

			Proxy = new ProxyProfile();

			//Not really necessary
			Privileges = new List<PrivilegeClass>();

			//Not really necessary
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
			if (target == null)
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

		public void Upgrade()
		{
			//This is used to upgrade old configuration formats
			if (Privileges.Count == 0)
			{
				//Unluckily lists default to empty lists instead of null so there is no nicer way to detect the necessity of an upgrade unless we stop using the current XML serialisation scheme without any further customisations
				PrivilegeClass privilegedClass = new PrivilegeClass();
				privilegedClass.EnabledAPIFunctions.AddRange(WebService.NonPrivilegedHandlers);
				privilegedClass.EnabledAPIFunctions.AddRange(WebService.PrivilegedHandlers);
				privilegedClass.Addresses.Add("127.0.0.1");

				PrivilegeClass nonPrivilegedClass = new PrivilegeClass();
				nonPrivilegedClass.EnabledAPIFunctions.AddRange(WebService.NonPrivilegedHandlers);
				nonPrivilegedClass.MatchAllAddresses = true;

				Privileges.Add(privilegedClass);
				Privileges.Add(nonPrivilegedClass);
			}

			//Old configuration formats didn't permit customised connection strings
			if (Database == "RiotControl.sqlite")
				Database = string.Format("Data Source = {0}", Database);
		}
	}
}
