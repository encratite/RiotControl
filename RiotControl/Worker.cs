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
		EngineRegionProfile Profile;

		Database Provider;
		DbConnection Connection;

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

		DatabaseCommand Command(string query, params object[] arguments)
		{
			return new DatabaseCommand(query, Connection, WorkerProfiler, arguments);
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
			DatabaseCommand currentValue = Command("select currval('{0}_id_seq')", tableName);
			object result = currentValue.ExecuteScalar();
			long id = (long)result;
			return (int)id;
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
