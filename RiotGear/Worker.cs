using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using System.Data.Common;

using LibOfLegends;

namespace RiotGear
{
	public partial class Worker
	{
		public EngineRegionProfile Profile
		{
			get;
			private set;
		}

		public RegionType Region
		{
			get;
			private set;
		}

		public bool Connected
		{
			get;
			private set;
		}

		bool Running;
		AutoResetEvent TerminationEvent;

		Thread AutomaticUpdatesThread;

		IGlobalHandler GlobalHandler;
		StatisticsService StatisticsService;
		Database Provider;

		Configuration Configuration;
		AuthenticationProfile AuthenticationProfile;
		RPCService RPC;

		Profiler Profiler;

		HashSet<int> ActiveAccountIds;

		int AutomaticUpdateInterval;

		public Worker(IGlobalHandler globalHandler, StatisticsService statisticsService, EngineRegionProfile regionProfile, Configuration configuration, Database provider)
		{
			Running = false;

			GlobalHandler = globalHandler;
			StatisticsService = statisticsService;
			Provider = provider;

			Configuration = configuration;
			Profile = regionProfile;

			AutomaticUpdatesThread = null;

			Connected = false;

			Profiler = new Profiler();
			ActiveAccountIds = new HashSet<int>();

			Region = (RegionType)Profile.Identifier;

			AutomaticUpdateInterval = configuration.AutomaticUpdateInterval;

			InitialiseAuthenticationProfile();
		}

		void InitialiseAuthenticationProfile()
		{
			//Create a new authentication profile that uses the client version from the master server instead of the (null) one provided by the configuration file
			AuthenticationProfile = new AuthenticationProfile();
			AuthenticationProfile.ClientVersion = Profile.ClientVersion;
			AuthenticationProfile.Domain = Configuration.Authentication.Domain;
			AuthenticationProfile.IPAddress = Configuration.Authentication.IPAddress;
			AuthenticationProfile.Locale = Configuration.Authentication.Locale;
		}

		DatabaseCommand Command(string query, DbConnection connection, params object[] arguments)
		{
			return new DatabaseCommand(query, connection, Profiler, arguments);
		}

		void WriteLine(string input, params object[] arguments)
		{
			lock (Profile)
			{
				if(Profile.Login != null)
					GlobalHandler.WriteLine(string.Format("[{0} {1}] {2}", Profile.Abbreviation, Profile.Login.Username, input), arguments);
				else
					GlobalHandler.WriteLine(string.Format("[{0}] {1}", Profile.Abbreviation, input), arguments);
			}
		}

		void SummonerMessage(string message, Summoner summoner, params object[] arguments)
		{
			WriteLine(string.Format("{0} ({1}): {2}", summoner.SummonerName, summoner.AccountId, message), arguments);
		}

		public void InitiateTermination()
		{
			lock (TerminationEvent)
			{
				Running = false;
				//Disconnecting the RPC service might actually cause FluorineFX threads to wait indefinitely for events, not entirely sure
				//Seems to work better without it
				//RPC.Disconnect();
				TerminationEvent.Set();
			}
			
		}

		public void WaitForUpdateThread()
		{
			try
			{
				lock (TerminationEvent)
				{
					if (AutomaticUpdatesThread != null)
					{
						AutomaticUpdatesThread.Join();
						AutomaticUpdatesThread = null;
					}
				}
			}
			catch (ThreadStateException)
			{
				//I think this means that the thread wasn't running
			}
		}

		public void Run()
		{
			Running = true;
			TerminationEvent = new AutoResetEvent(false);
			Connect();
		}	

		void Connect()
		{
			//Obtain a lock on the profile to avoid race conditions while the user is editing the data
			lock (Profile)
			{
				if (Profile.Login == null)
				{
					//The user has removed the login for this worker after the worker had been previously connecting - cancel
					WriteLine("No login specified");
					return;
				}
				else
				{
					ConnectionProfile connectionData = new ConnectionProfile(AuthenticationProfile, Profile.Region, Configuration.Proxy, Profile.Login.Username.ToLower(), Profile.Login.Password);
					RPC = new RPCService(connectionData, OnConnect, OnDisconnect);
					WriteLine("Connecting to the server");
				}
			}
			RPC.Connect();
		}

		void ConnectInThread()
		{
			if (Running)
			{
				Thread thread = new Thread(Connect);
				thread.Name = string.Format("{0} Connection Thread", Profile.Description);
				thread.Start();
			}
		}

		void OnConnect(RPCConnectResult result)
		{
			try
			{
				if (result.Success())
				{
					Connected = true;
					WriteLine("Successfully connected to the server");
					TerminateUpdateThread();
					lock (TerminationEvent)
					{
						AutomaticUpdatesThread = new Thread(RunAutomaticUpdates);
						AutomaticUpdatesThread.Name = string.Format("{0} Automatic updates", Profile.Description);
						AutomaticUpdatesThread.Start();
					}
				}
				else
				{
					if(result.Result == RPCConnectResultType.LoginFault && result.FlexLoginFault.FaultString == "com.riotgames.platform.login.LoginFailedException : null")
						WriteLine("The server has placed this client in the login queue. Please be patient.");
					else
						WriteLine(result.GetMessage());
					TerminationEvent.WaitOne(Configuration.ReconnectDelay);
					ConnectInThread();
				}
			}
			catch (Exception exception)
			{
				GlobalHandler.HandleException(exception);
			}
		}

		void TerminateUpdateThread()
		{
			lock (TerminationEvent)
			{
				TerminationEvent.Set();
				WaitForUpdateThread();
				if (Running)
					TerminationEvent.Reset();
			}
		}

		void OnDisconnect()
		{
			//You get disconnected after idling for two hours
			Connected = false;
			WriteLine("Disconnected");
			if (Running)
			{
				//Shut down the automatic update thread and reconnect
				TerminateUpdateThread();
				TerminationEvent.WaitOne(Configuration.ReconnectDelay);
				ConnectInThread();
			}
		}

		string GetGroupString(string[] fields)
		{
			return String.Join(", ", fields);
		}

		string GetPlaceholderString(string[] fields)
		{
			var mapped = from x in fields
						 select string.Format("{0}{1}", Provider.GetParameterPrefix(), x);
			return GetGroupString(mapped.ToArray());
		}

		string GetUpdateString(string[] fields)
		{
			var mapped = from x in fields
						 select string.Format("{0} = {1}{0}", x, Provider.GetParameterPrefix());
			return GetGroupString(mapped.ToArray());
		}

		int GetInsertId(string table, DbConnection connection)
		{
			switch(Provider.Type)
			{
				case DatabaseType.SQLite:
					return (int)(long)Command("select last_insert_rowid()", connection).ExecuteScalar();

				case DatabaseType.PostgreSQL:
					return (int)(long)Command("select currval('{0}_id_seq')", connection, table).ExecuteScalar();

				case DatabaseType.MySQL:
					return (int)(long)Command("select last_insert_id()", connection, table).ExecuteScalar();

				default:
					throw new Exception("Unable to retrieve the last insert ID because this is not a known database provider");
			}
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

		bool PerformUpdates()
		{
			return Running && Connected;
		}

		void RunAutomaticUpdates()
		{
			while (PerformUpdates())
			{
				List<Summoner> summoners = StatisticsService.GetAutomaticUpdateSummoners(Region);
				if (summoners.Count > 0)
					WriteLine("Performing automatic updates for {0} summoner(s)", summoners.Count);
				foreach (var summoner in summoners)
				{
					if (!PerformUpdates())
						return;
					WriteLine("Performing automatic updates for summoner {0}", summoner.SummonerName);
					OperationResult result = UpdateSummonerByAccountId(summoner.AccountId);
					if (result != OperationResult.Success && result != OperationResult.NotFound)
						WriteLine("Update for summoner {0} failed", summoner.SummonerName);
				}
				if (summoners.Count > 0)
					WriteLine("Done performing automatic updates for {0} summoner(s)", summoners.Count);
				TerminationEvent.WaitOne(AutomaticUpdateInterval * 1000);
			}
		}
	}
}
