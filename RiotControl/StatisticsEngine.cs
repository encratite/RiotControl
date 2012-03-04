using System;
using System.Collections.Generic;
using System.Threading;

using Npgsql;

namespace RiotControl
{
	class StatisticsEngine
	{
		Configuration EngineConfiguration;
		AutoResetEvent TerminationEvent;
		List<RegionHandler> RegionHandlers;

		public StatisticsEngine(Configuration configuration)
		{
			EngineConfiguration = configuration;
			TerminationEvent = new AutoResetEvent(false);
		}

		public void RunEngine()
		{
			CreateRegionHandlers();
			TerminationEvent.WaitOne();
			//It would be nice to shut down workers gracefully here, too
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
