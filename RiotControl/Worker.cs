using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Npgsql;
using NpgsqlTypes;

using LibOfLegends;

using com.riotgames.platform.statistics;
using com.riotgames.platform.summoner;

namespace RiotControl
{
	partial class Worker
	{
		EngineRegionProfile RegionProfile;
		Login WorkerLogin;
		NpgsqlConnection Database;
		RPCService RPC;

		Profiler WorkerProfiler;

		RegionHandler Master;

		AutoResetEvent JobEvent;

		public Worker(Configuration configuration, EngineRegionProfile regionProfile, Login login, RegionHandler regionHandler)
		{
			RegionProfile = regionProfile;
			WorkerLogin = login;

			WorkerProfiler = new Profiler();

			JobEvent = new AutoResetEvent(false);

			Master = regionHandler;

			InitialiseDatabase(configuration.Database);
			ConnectionProfile connectionData = new ConnectionProfile(configuration.Authentication, regionProfile.Region, configuration.Proxy, login.Username, login.Password);
			RPC = new RPCService(connectionData);
			WriteLine("Connecting to the server");
			RPC.Connect(OnConnect);
		}

		void InitialiseDatabase(DatabaseConfiguration databaseConfiguration)
		{
			Database = new NpgsqlConnection("Server = " + databaseConfiguration.Host + "; Port = " + databaseConfiguration.Port + "; User Id = " + databaseConfiguration.Username + "; Database = " + databaseConfiguration.Database + "; Preload Reader = true; Pooling = true; Minpoolsize = " + databaseConfiguration.MinimumPoolSize + "; Maxpoolsize = " + databaseConfiguration.MaximumPoolSize + ";");
			try
			{
				Database.Open();
			}
			catch (Exception exception)
			{
				Console.WriteLine("Unable to connect to SQL server: " + exception);
				return;
			}
		}

		SQLCommand Command(string query, params object[] arguments)
		{
			return new SQLCommand(query, Database, WorkerProfiler, arguments);
		}

		void WriteLine(string input, params object[] arguments)
		{
			Nil.Output.WriteLine(string.Format("{0} [{1} {2}] {3}", Nil.Time.Timestamp(), RegionProfile.Abbreviation, WorkerLogin.Username, input), arguments);
		}

		void SummonerMessage(string message, Summoner summoner, params object[] arguments)
		{
			WriteLine(string.Format("{0} ({1}): {2}", summoner.Name, summoner.AccountId, message), arguments);
		}

		void OnConnect(RPCConnectResult result)
		{
			if (result.Success())
			{
				WriteLine("Successfully connected to the server");
				//OnConnect must return so we can't use the current thread to execute region handler logic.
				//This is a limitation imposed by FluorineFX.
				(new Thread(Run)).Start();
			}
			else
				WriteLine(result.GetMessage());
		}

		string GetGroupString(List<string> fields)
		{
			return String.Join(", ", fields);
		}

		string GetPlaceholderString(List<string> fields)
		{
			var mapped = from x in fields
						 select ":" + x;
			return GetGroupString(mapped.ToList());
		}

		string Zulufy(string input)
		{
			return string.Format("{0} at time zone 'UTC'", input);
		}

		string CurrentTimestamp()
		{
			return Zulufy("current_timestamp");
		}

		int GetInsertId(string tableName)
		{
			SQLCommand currentValue = Command("select currval('{0}_id_seq')", tableName);
			object result = currentValue.ExecuteScalar();
			long id = (long)result;
			return (int)id;
		}

		//This method tells the worker that a new job has been added and should attempt to process jobs from the queues
		public void Notify()
		{
			JobEvent.Set();
		}

		void Run()
		{
			while (true)
			{
				LookupJob lookupJob = Master.GetLookupJob();
				if (lookupJob != null)
				{
					throw new Exception("Not implemented");
				}

				UpdateJob manualUpdateJob = Master.GetManualUpdateJob();
				if (manualUpdateJob != null)
				{
					throw new Exception("Not implemented");
				}

				UpdateJob automaticUpdateJob = Master.GetAutomaticUpdateJob();
				if (automaticUpdateJob != null)
				{
					throw new Exception("Not implemented");
				}

				//No jobs are available right now, wait for the next one
				JobEvent.WaitOne();
			}
		}
	}
}
