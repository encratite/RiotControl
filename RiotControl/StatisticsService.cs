using System.Collections.Generic;

namespace RiotControl
{
	class StatisticsService
	{
		Configuration EngineConfiguration;
		List<RegionHandler> RegionHandlers;

		public StatisticsService(Configuration configuration)
		{
			EngineConfiguration = configuration;
		}

		public void Run()
		{
			CreateRegionHandlers();
		}

		void CreateRegionHandlers()
		{
			RegionHandlers = new List<RegionHandler>();
			foreach (var profile in EngineConfiguration.RegionProfiles)
			{
				RegionHandler handler = new RegionHandler(EngineConfiguration, profile);
				RegionHandlers.Add(handler);
			}
		}
	}
}
