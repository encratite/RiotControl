using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Nil;

namespace RiotGear
{
	public partial class StatisticsService
	{
		IGlobalHandler GlobalHandler;
		Configuration Configuration;
		Database Provider;
		//Maps region abbreviations to workers
		Dictionary<string, Worker> Workers;
		Dictionary<RegionType, Dictionary<int, Summoner>> SummonerCache;

		public StatisticsService(IGlobalHandler globalhandler, Configuration configuration, Database databaseProvider)
		{
			Workers = new Dictionary<string, Worker>();
			GlobalHandler = globalhandler;
			Configuration = configuration;
			Provider = databaseProvider;
		}

		public void Run()
		{
			UpgradeDatabase();
			InitialiseSummonerCache();

			if (!InitialiseClientVersions())
				return;
			AddMissingWorkers();
		}

		//This method is used to add new workers once new logins have been defined
		public void AddMissingWorkers()
		{
			//Obtain a lock on the workers, since the user might be causing this to be called concurrently from the GUI and from the startup procedure
			lock (Workers)
			{
				foreach (var profile in Configuration.RegionProfiles)
				{
					//Obtain a lock on this profile because it is concurrently being modified by the user in the edit dialogue
					lock (profile)
					{
						if (profile.Login == null)
						{
							//No login has been specified for this region
							continue;
						}
						if (Workers.ContainsKey(profile.Abbreviation))
						{
							//There is already a worker for this region, may it be active or inactive - do not proceed
							continue;
						}
						Worker worker = new Worker(GlobalHandler, this, profile, Configuration, Provider);
						Workers[profile.Abbreviation] = worker;
						worker.Run();
					}
				}
			}
		}

		bool InitialiseClientVersions()
		{
			WebClient client = new WebClient();
			try
			{
				string contents = client.DownloadString(Configuration.ClientVersionsURL);
				List<string> lines = contents.Tokenise("\n");
				if (lines.Count != Configuration.RegionProfiles.Count)
				{
					WriteLine("The number of client versions provided by the master server and the number of region profiles in the application's configuration file do not match");
					return false;
				}
				foreach (var line in lines)
				{
					List<string> tokens = line.Tokenise(" ");
					if (tokens.Count != 2)
					{
						WriteLine("Invalid number of tokens in a line of the client versions file");
						return false;
					}
					bool hit = false;
					string abbreviation = tokens[0];
					string clientVersion = tokens[1];
					foreach (var profile in Configuration.RegionProfiles)
					{
						if (profile.Abbreviation == abbreviation)
						{
							profile.ClientVersion = clientVersion;
							WriteLine("Client version of {0}: {1}", profile.Description, clientVersion);
							hit = true;
							break;
						}
					}
					if (!hit)
					{
						WriteLine("Unable to find a matching local region profile for a client version provided by the master server");
						return false;
					}
				}
				foreach (var profile in Configuration.RegionProfiles)
				{
					if (profile.ClientVersion == null)
					{
						WriteLine("The server didn't specify a client version for region {0}", profile.Description);
						return false;
					}
				}
				return true;
			}
			catch (WebException exception)
			{
				WriteLine("The download of the client versions from the master server failed: {0}", exception.Message);
				return false;
			}
		}

		public Worker GetWorkerByAbbreviation(string abbreviation)
		{
			lock (Workers)
			{
				foreach (var worker in Workers.Values)
				{
					lock (worker.Profile)
					{
						if (worker.Profile.Abbreviation == abbreviation)
							return worker;
					}
				}
			}
			throw new Exception("No such region");
		}

		void InitialiseSummonerCache()
		{
			SummonerCache = new Dictionary<RegionType, Dictionary<int, Summoner>>();

			foreach (RegionType regionType in Enum.GetValues(typeof(RegionType)))
				SummonerCache[regionType] = new Dictionary<int, Summoner>();


			using (var connection = Provider.GetConnection())
			{
				using (var select = new DatabaseCommand("select {0} from summoner", connection, null, Summoner.GetFields()))
				{
					using (var reader = select.ExecuteReader())
					{
						while (reader.Read())
						{
							Summoner summoner = new Summoner(reader);
							SummonerCache[summoner.Region][summoner.AccountId] = summoner;
						}
					}
				}
			}
		}

		public Summoner GetSummoner(RegionType region, int accountId)
		{
			lock (SummonerCache)
			{
				Summoner output = null;
				SummonerCache[region].TryGetValue(accountId, out output);
				return output;
			}
		}

		public Summoner GetSummoner(RegionType region, string name)
		{
			//This is bad because the name search could be performed much faster, but as this code was already modified a lot to be specialised for a much smaller database it probably won't have much of an impact right now
			lock (SummonerCache)
			{
				string target = name.ToLower();
				foreach (var summoner in SummonerCache[region].Values)
				{
					if (target == summoner.SummonerName.ToLower())
						return summoner;
				}
				return null;
			}
		}

		public void AddSummonerToCache(RegionType region, Summoner summoner)
		{
			lock (SummonerCache)
			{
				SummonerCache[summoner.Region][summoner.AccountId] = summoner;
			}
		}

		public List<Summoner> GetAutomaticUpdateSummoners(RegionType region)
		{
			lock (SummonerCache)
			{
				var regionCache = SummonerCache[region];
				return (from x in regionCache.Values where x.UpdateAutomatically == true select x).ToList();
			}
		}

		public List<EngineRegionProfile> GetActiveProfiles()
		{
			lock (Workers)
			{
				List<EngineRegionProfile> output = new List<EngineRegionProfile>();
				foreach (var worker in Workers.Values)
				{
					//Obtain a lock to avoid race conditions with the other code that modifies worker profiles
					lock (worker.Profile)
					{
						if (worker.Connected)
							output.Add(worker.Profile);
					}
				}
				return output;
			}
		}

		void WriteLine(string message, params object[] arguments)
		{
			GlobalHandler.WriteLine(message, arguments);
		}

		public void Terminate()
		{
			lock (Workers)
			{
				var workers = Workers.Values;

				//Initiate termination first so all automatic update threads are shut down simultaneously
				foreach (var worker in workers)
					worker.InitiateTermination();

				//Now wait for them to actually terminate
				foreach (var worker in workers)
					worker.WaitForTermination();
			}
		}
	}
}
