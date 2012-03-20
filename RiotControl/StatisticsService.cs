using System.Collections.Generic;

namespace RiotControl
{
	class StatisticsService
	{
		Configuration ServiceConfiguration;
		Database Provider;
		List<Worker> Workers;

		public StatisticsService(Configuration configuration, Database databaseProvider)
		{
			ServiceConfiguration = configuration;
			Provider = databaseProvider;
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
				Worker worker = new Worker(profile, ServiceConfiguration, Provider);
				Workers.Add(worker);
			}
		}
	}
}
