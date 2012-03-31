using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using System.Data.Common;

using LibOfLegends;

namespace RiotControl
{
	partial class Worker
	{
		public EngineRegionProfile WorkerProfile
		{
			get
			{
				return Profile;
			}
		}

		public RegionType WorkerRegion
		{
			get
			{
				return Region;
			}
		}

		RiotControl RiotControl;

		StatisticsService StatisticsService;

		EngineRegionProfile Profile;
		RegionType Region;

		Database Provider;

		RPCService RPC;

		Profiler WorkerProfiler;

		ConnectionProfile ConnectionData;

		bool Connected;

		HashSet<int> ActiveAccountIds;

		int AutomaticUpdateInterval;

		public Worker(RiotControl riotControl, StatisticsService statisticsService, EngineRegionProfile regionProfile, Configuration configuration, Database provider)
		{
			RiotControl = riotControl;
			StatisticsService = statisticsService;
			Profile = regionProfile;
			Provider = provider;

			Connected = false;

			WorkerProfiler = new Profiler();
			ActiveAccountIds = new HashSet<int>();

			Region = (RegionType)Profile.Identifier;

			Login login = Profile.Login;

			ConnectionData = new ConnectionProfile(configuration.Authentication, regionProfile.Region, configuration.Proxy, login.Username, login.Password);
			AutomaticUpdateInterval = configuration.AutomaticUpdateInterval;
		}

		DatabaseCommand Command(string query, DbConnection connection, params object[] arguments)
		{
			return new DatabaseCommand(query, connection, WorkerProfiler, arguments);
		}

		void WriteLine(string input, params object[] arguments)
		{
			RiotControl.WriteLine(string.Format("{0} [{1} {2}] {3}", Nil.Time.Timestamp(), Profile.Abbreviation, Profile.Login.Username, input), arguments);
		}

		void SummonerMessage(string message, Summoner summoner, params object[] arguments)
		{
			WriteLine(string.Format("{0} ({1}): {2}", summoner.SummonerName, summoner.AccountId, message), arguments);
		}

		public void Run()
		{
			Connect();
		}	

		void Connect()
		{
			RPC = new RPCService(ConnectionData, OnConnect, OnDisconnect);
			WriteLine("Connecting to the server");
			RPC.Connect();
		}

		void ConnectInThread()
		{
			(new Thread(Connect)).Start();
		}

		void OnConnect(RPCConnectResult result)
		{
			if (result.Success())
			{
				Connected = true;
				WriteLine("Successfully connected to the server");
				(new Thread(RunAutomaticUpdates)).Start();
			}
			else
			{
				WriteLine(result.GetMessage());
				//Just reconnect right away
				//This is a bit of a hack, required to make this work with Mono because connections will just randomly fail there
				ConnectInThread();
			}
		}

		void OnDisconnect()
		{
			//You get disconnected after idling for two hours
			Connected = false;
			WriteLine("Disconnected");
			//Reconnect
			Thread.Sleep(5000);
			ConnectInThread();
		}

		string GetGroupString(string[] fields)
		{
			return String.Join(", ", fields);
		}

		string GetPlaceholderString(string[] fields)
		{
			var mapped = from x in fields
						 select string.Format(":{0}", x);
			return GetGroupString(mapped.ToArray());
		}

		string GetUpdateString(string[] fields)
		{
			var mapped = from x in fields
						 select string.Format("{0} = :{0}", x);
			return GetGroupString(mapped.ToArray());
		}

		int GetInsertId(DbConnection connection)
		{
			return (int)(long)Command("select last_insert_rowid()", connection).ExecuteScalar();
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

		void RunAutomaticUpdates()
		{
			while (Connected)
			{
				List<Summoner> summoners = StatisticsService.GetAutomaticUpdateSummoners(Region);
				if (summoners.Count > 0)
					WriteLine("Performing automatic updates for {0} summoner(s)", summoners.Count);
				foreach (var summoner in summoners)
				{
					WriteLine("Performing automatic updates for summoner " + summoner.SummonerName);
					OperationResult result = UpdateSummonerByAccountId(summoner.AccountId);
					if (!Connected)
						break;
					if (result != OperationResult.Success && result != OperationResult.NotFound)
					{
						//There might be something fishy going on with the connection, delay the next operation
						Thread.Sleep(10000);
					}
				}
				if (summoners.Count > 0)
					WriteLine("Done performing automatic updates for {0} summoner(s)", summoners.Count);
				if (Connected)
					Thread.Sleep(AutomaticUpdateInterval * 1000);
			}
		}
	}
}
