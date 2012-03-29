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


		StatisticsService Master;

		EngineRegionProfile Profile;
		RegionType Region;

		Database Provider;

		RPCService RPC;

		Profiler WorkerProfiler;

		ConnectionProfile ConnectionData;

		bool Connected;

		HashSet<int> ActiveAccountIds;

		public Worker(StatisticsService master, EngineRegionProfile regionProfile, Configuration configuration, Database provider)
		{
			Master = master;
			Profile = regionProfile;
			Provider = provider;

			Connected = false;

			WorkerProfiler = new Profiler();
			ActiveAccountIds = new HashSet<int>();

			Region = (RegionType)Profile.Identifier;

			Login login = Profile.Login;

			ConnectionData = new ConnectionProfile(configuration.Authentication, regionProfile.Region, configuration.Proxy, login.Username, login.Password);
			Connect();
		}

		DatabaseCommand Command(string query, DbConnection connection, params object[] arguments)
		{
			return new DatabaseCommand(query, connection, WorkerProfiler, arguments);
		}

		void WriteLine(string input, params object[] arguments)
		{
			Nil.Output.WriteLine(string.Format("{0} [{1} {2}] {3}", Nil.Time.Timestamp(), Profile.Abbreviation, Profile.Login.Username, input), arguments);
		}

		void SummonerMessage(string message, Summoner summoner, params object[] arguments)
		{
			WriteLine(string.Format("{0} ({1}): {2}", summoner.SummonerName, summoner.AccountId, message), arguments);
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
	}
}
