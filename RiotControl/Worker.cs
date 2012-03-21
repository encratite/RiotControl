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

		EngineRegionProfile Profile;

		Database Provider;
		DbConnection Connection;

		RPCService RPC;

		Profiler WorkerProfiler;

		ConnectionProfile ConnectionData;

		bool Connected;

		HashSet<int> ActiveAccountIds;

		public Worker(EngineRegionProfile regionProfile, Configuration configuration, Database provider)
		{
			Profile = regionProfile;
			Provider = provider;

			Connected = false;

			WorkerProfiler = new Profiler();
			ActiveAccountIds = new HashSet<int>();

			Connection = Provider.GetConnection();
			ConnectionData = new ConnectionProfile(configuration.Authentication, regionProfile.Region, configuration.Proxy, Profile.Username, Profile.Password);
			Connect();
		}

		DatabaseCommand Command(string query, params object[] arguments)
		{
			return new DatabaseCommand(query, Connection, WorkerProfiler, arguments);
		}

		void WriteLine(string input, params object[] arguments)
		{
			Nil.Output.WriteLine(string.Format("{0} [{1} {2}] {3}", Nil.Time.Timestamp(), Profile.Abbreviation, Profile.Username, input), arguments);
		}

		void SummonerMessage(string message, SummonerDescription summoner, params object[] arguments)
		{
			WriteLine(string.Format("{0} ({1}): {2}", summoner.Name, summoner.AccountId, message), arguments);
		}

		void Connect()
		{
			RPC = new RPCService(ConnectionData, OnConnect, OnDisconnect);
			WriteLine("Connecting to the server");
			RPC.Connect();
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
				(new Thread(Connect)).Start();
			}
		}

		void OnDisconnect()
		{
			Connected = false;
			WriteLine("Disconnected");
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

		int GetInsertId()
		{
			return (int)(long)Command("select last_insert_rowid()").ExecuteScalar();
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
