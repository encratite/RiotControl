using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Nil;
using Npgsql;
using LibOfLegends;

namespace RiotControl
{
	class RegionHandler
	{
		EngineRegionProfile RegionProfile;
		NpgsqlConnection Database;
		RPCService RPC;

		public RegionHandler(Configuration configuration, EngineRegionProfile regionProfile, NpgsqlConnection database)
		{
			RegionProfile = regionProfile;
			Database = database;
			if (regionProfile.Logins.Count != 1)
				throw new Exception("Currently the number of accounts per region is limited to one");
			Login login = regionProfile.Logins.First();
			ConnectionProfile connectionData = new ConnectionProfile(configuration.Authentication, regionProfile.Region, configuration.Proxy, login.Username, login.Password);
			RPC = new RPCService(connectionData);
			RPC.Connect(OnConnect);
		}

		void WriteLine(string input)
		{
			Output.WriteLine("[" + RegionProfile.Abbreviation + "] " + input);
		}

		void OnConnect(bool connected)
		{
			if (connected)
			{
				WriteLine("Successfully connected to the server.");
				//OnConnect must return so we can't use the current thread to execute region handler logic.
				//This is a limitation imposed by FluorineFX.
				(new Thread(Run)).Start();
			}
			else
				WriteLine("There was an error connecting to the server.");
		}

		void Run()
		{
			while (true)
			{
			}
		}
	}
}
