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
		List<Worker> RegionHandlers;

		public StatisticsEngine(Configuration configuration)
		{
			EngineConfiguration = configuration;
			TerminationEvent = new AutoResetEvent(false);
		}

		public void RunEngine()
		{
			CreateRegionHandlers();
			TerminationEvent.WaitOne();
			foreach (var handler in RegionHandlers)
				handler.CloseDatabase();
		}

		void CreateRegionHandlers()
		{
			RegionHandlers = new List<Worker>();
			foreach (var profile in EngineConfiguration.RegionProfiles)
			{
				Worker handler = new Worker(EngineConfiguration, profile, EngineConfiguration.Database);
				RegionHandlers.Add(handler);
			}
		}
	}
}
