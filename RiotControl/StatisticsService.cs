using System.Collections.Generic;

namespace RiotControl
{
	class StatisticsService
	{
		Configuration ServiceConfiguration;
		DatabaseConnectionProvider DatabaseProvider;
		List<RegionHandler> RegionHandlers;

		public StatisticsService(Configuration configuration, DatabaseConnectionProvider databaseProvider)
		{
			ServiceConfiguration = configuration;
			DatabaseProvider = databaseProvider;
		}

		public void Run()
		{
			CreateRegionHandlers();
		}

		void CreateRegionHandlers()
		{
			RegionHandlers = new List<RegionHandler>();
			foreach (var profile in ServiceConfiguration.RegionProfiles)
			{
				RegionHandler handler = new RegionHandler(ServiceConfiguration, profile, DatabaseProvider);
				RegionHandlers.Add(handler);
			}
		}

		public RegionHandler GetRegionHandler(string abbreviation)
		{
			foreach (var handler in RegionHandlers)
			{
				if (handler.MatchesAbbreviation(abbreviation))
					return handler;
			}
			return null;
		}

		public List<EngineRegionProfile> GetRegionProfiles()
		{
			List<EngineRegionProfile> output = new List<EngineRegionProfile>();
			foreach (var handler in RegionHandlers)
				output.Add(handler.HandlerProfile);
			return output;
		}
	}
}
