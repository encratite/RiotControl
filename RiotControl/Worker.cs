using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using System.Data.SQLite;

using LibOfLegends;

using com.riotgames.platform.statistics;
using com.riotgames.platform.summoner;
using com.riotgames.platform.gameclient.domain;

namespace RiotControl
{
	partial class Worker
	{
		EngineRegionProfile Profile;

		Database Provider;
		SQLiteConnection Connection;

		RPCService RPC;

		Profiler WorkerProfiler;

		ConnectionProfile ConnectionData;

		public Worker(EngineRegionProfile regionProfile, Configuration configuration, Database provider)
		{
			Profile = regionProfile;

			WorkerProfiler = new Profiler();

			Provider = provider;

			Connection = Provider.GetConnection();
			ConnectionData = new ConnectionProfile(configuration.Authentication, regionProfile.Region, configuration.Proxy, Profile.Username, Profile.Password);
			Connect();
		}

		SQLCommand Command(string query, params object[] arguments)
		{
			return new SQLCommand(query, Connection, WorkerProfiler, arguments);
		}

		void WriteLine(string input, params object[] arguments)
		{
			Nil.Output.WriteLine(string.Format("{0} [{1} {2}] {3}", Nil.Time.Timestamp(), Profile.Abbreviation, Username, input), arguments);
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
			{
				WriteLine(result.GetMessage());
				//Just reconnect right away
				//This is a bit of a hack, required to make this work with Mono because connections will just randomly fail there
				(new Thread(Connect)).Start();
			}
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

		void ProcessAccountIdJob(AccountIdJob job)
		{
			SQLCommand nameLookup = Command("select id, account_id, summoner_name from summoner where region = cast(:region as region_type) and account_id = :account_id");
			nameLookup.SetEnum("region", Profile.RegionEnum);
			nameLookup.Set("account_id", job.AccountId);
			using (NpgsqlDataReader nameReader = nameLookup.ExecuteReader())
			{
				if (nameReader.Read())
				{
					int id = (int)nameReader[0];
					int accountId = (int)nameReader[1];
					string name = (string)nameReader[2];
					UpdateSummoner(new SummonerDescription(name, id, accountId), false);
					job.ProvideResult(JobQueryResult.Success);
				}
				else
				{
					//The account isn't in the database yet, add it
					AllPublicSummonerDataDTO publicSummonerData = RPC.GetAllPublicSummonerDataByAccount(job.AccountId);
					if (publicSummonerData != null)
					{
						var summoner = publicSummonerData.summoner;
						int id = InsertNewSummoner(summoner.acctId, summoner.sumId, summoner.name, summoner.internalName, publicSummonerData.summonerLevel.summonerLevel, summoner.profileIconId);
						UpdateSummoner(new SummonerDescription(summoner.name, id, summoner.acctId), false);
						job.ProvideResult(JobQueryResult.Success);
					}
					else
					{
						//No such summoner
						job.ProvideResult(JobQueryResult.NotFound);
					}
				}
			}
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

				AccountIdJob idJob = Master.GetManualUpdateJob();
				if (idJob == null)
					idJob = Master.GetAutomaticUpdateJob();
				if (idJob != null)
				{
					try
					{
						ProcessAccountIdJob(idJob);
						continue;
					}
					catch (RPCTimeoutException)
					{
						idJob.ProvideResult(JobQueryResult.Timeout);
						Timeout();
						break;
					}
				}

				//No jobs are available right now, wait for the next one
				//WriteLine("Waiting for a job");
				JobEvent.WaitOne();
			}
		}
	}
}
