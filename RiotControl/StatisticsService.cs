using System;
using System.Collections.Generic;
using System.Linq;

namespace RiotControl
{
	class StatisticsService
	{
		RiotControl RiotControl;
		Configuration ServiceConfiguration;
		Database Provider;
		List<Worker> Workers;
		Dictionary<RegionType, Dictionary<int, Summoner>> SummonerCache;

		public StatisticsService(RiotControl riotControl, Configuration configuration, Database databaseProvider)
		{
			RiotControl = riotControl;
			ServiceConfiguration = configuration;
			Provider = databaseProvider;

			InitialiseSummonerCache();
		}

		public void Run()
		{
			CreateWorkers();
		}

		void CreateWorkers()
		{
			Workers = new List<Worker>();
			foreach (var profile in ServiceConfiguration.RegionProfiles)
			{
				//Check if a login has been specified for this region
				if (profile.Login == null)
					continue;
				Worker worker = new Worker(RiotControl, this, profile, ServiceConfiguration, Provider);
				Workers.Add(worker);
				worker.Run();
			}
		}

		public List<EngineRegionProfile> GetRegionProfiles()
		{
			var output = from x in Workers select x.WorkerProfile;
			return output.ToList();
		}

		public bool GetRegionIdentifier(string abbreviation, ref int identifier)
		{
			foreach (var worker in Workers)
			{
				if (worker.WorkerProfile.Abbreviation == abbreviation)
				{
					identifier = worker.WorkerProfile.Identifier;
					return true;
				}

			}
			return false;
		}

		public Worker GetWorkerByAbbreviation(string abbreviation)
		{
			foreach (var worker in Workers)
			{
				if (worker.WorkerProfile.Abbreviation == abbreviation)
					return worker;
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
	}
}
