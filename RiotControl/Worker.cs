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

		ConnectionProfile ConnectionData;

		public Worker(Configuration configuration, EngineRegionProfile regionProfile, Login login, RegionHandler regionHandler)
		{
			RegionProfile = regionProfile;
			WorkerLogin = login;

			WorkerProfiler = new Profiler();

			JobEvent = new AutoResetEvent(false);

			Master = regionHandler;

			InitialiseDatabase(configuration.Database);
			ConnectionData = new ConnectionProfile(configuration.Authentication, regionProfile.Region, configuration.Proxy, login.Username, login.Password);
			Connect();
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

		void SummonerMessage(string message, SummonerDescription summoner, params object[] arguments)
		{
			WriteLine(string.Format("{0} ({1}): {2}", summoner.Name, summoner.AccountId, message), arguments);
		}

		void Connect()
		{
			RPC = new RPCService(ConnectionData, OnConnect);
			WriteLine("Connecting to the server");
			RPC.Connect();
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

		void Reconnect()
		{
			RPC.Disconnect();
			Connect();
		}

		void Timeout()
		{
			WriteLine("A remote call has timed out, attempting to reconnect");
			Reconnect();
		}

		void ProcessUpdateJob(UpdateJob job)
		{
			SQLCommand nameLookup = Command("select id, account_id, summoner_name from summoner where region = cast(:region as region_type) and account_id = :account_id");
			nameLookup.SetEnum("region", RegionProfile.RegionEnum);
			nameLookup.Set("account_id", job.AccountId);
			NpgsqlDataReader nameReader = nameLookup.ExecuteReader();
			if (nameReader.Read())
			{
				int id = (int)nameReader[0];
				int accountId = (int)nameReader[1];
				string name = (string)nameReader[2];
				UpdateSummoner(new SummonerDescription(name, id, accountId), false);
				job.ProvideResult(JobQueryResult.Success);
			}
			else
				job.ProvideResult(JobQueryResult.NotFound);
		}

		void Run()
		{
			while (true)
			{
				LookupJob lookupJob = Master.GetLookupJob();
				if (lookupJob != null)
				{
					try
					{
						UpdateSummonerByName(lookupJob);
						continue;
					}
					catch (RPCTimeoutException)
					{
						lookupJob.ProvideResult(JobQueryResult.Timeout);
						Timeout();
						break;
					}
				}

				UpdateJob updateJob = Master.GetManualUpdateJob();
				if (updateJob == null)
					updateJob = Master.GetAutomaticUpdateJob();
				if (updateJob != null)
				{
					try
					{
						ProcessUpdateJob(updateJob);
						continue;
					}
					catch (RPCTimeoutException)
					{
						updateJob.ProvideResult(JobQueryResult.Timeout);
						Timeout();
						break;
					}
				}

				//No jobs are available right now, wait for the next one
				WriteLine("Waiting for a job");
				JobEvent.WaitOne();
			}
		}
	}
}
