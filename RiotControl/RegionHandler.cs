using System;
using System.Linq;
using System.Threading;

using LibOfLegends;
using Nil;
using Npgsql;
using NpgsqlTypes;

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
			WriteLine("Connecting to the server");
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
				WriteLine("Successfully connected to the server");
				//OnConnect must return so we can't use the current thread to execute region handler logic.
				//This is a limitation imposed by FluorineFX.
				(new Thread(Run)).Start();
			}
			else
				WriteLine("There was an error connecting to the server");
		}

		void Run()
		{
			NpgsqlCommand command = new NpgsqlCommand("select id, region, summoner_name from lookup_job where region = cast(:region as region_type) order by priority desc, time_added limit 1", Database);
			command.Parameters.Add(new NpgsqlParameter("region", NpgsqlDbType.Varchar));
			command.Parameters[0].Value = RegionProfile.RegionEnum;
			while (true)
			{
				NpgsqlDataReader reader = command.ExecuteReader();
				while (reader.Read())
				{
					int id = (int)reader[0];
					RegionType region = ((string)reader[1]).RegionStringToEnum();
					string summonerName = (string)reader[2];
				}
			}
		}
	}
}
