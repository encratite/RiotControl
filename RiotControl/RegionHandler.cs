using System;
using System.Linq;
using System.Threading;

using LibOfLegends;
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

		void WriteLine(string input, params object[] arguments)
		{
			Nil.Output.WriteLine(Nil.Time.Timestamp() + " [" + RegionProfile.Abbreviation + "] " + input, arguments);
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

		void LookupSummoner(string summonerName)
		{
		}

		void Run()
		{
			NpgsqlCommand selectCommand = new NpgsqlCommand("select id, summoner_name from lookup_job where region = cast(:region as region_type) order by priority desc, time_added limit 1", Database);
			selectCommand.Parameters.Add(new NpgsqlParameter("region", NpgsqlDbType.Varchar));
			selectCommand.Parameters[0].Value = RegionProfile.RegionEnum;

			NpgsqlCommand deleteCommand = new NpgsqlCommand("delete from lookup_job where id = :id", Database);
			deleteCommand.Parameters.Add(new NpgsqlParameter(":id", NpgsqlDbType.Integer));

			while (true)
			{
				NpgsqlTransaction lookupTransaction = Database.BeginTransaction();
				NpgsqlDataReader reader = selectCommand.ExecuteReader();
				bool success = reader.Read();

				int id;
				String summonerName = null;

				if (success)
				{
					id = (int)reader[0];
					summonerName = (string)reader[1];

					//Delete entry
					//deleteCommand.Parameters[0].Value = id;
					//deleteCommand.ExecuteNonQuery();
				}

				lookupTransaction.Commit();

				if (success)
					LookupSummoner(summonerName);
				else
				{
					WriteLine("No jobs available");
					//Should wait for an event here really
					break;
				}
			}
		}
	}
}
