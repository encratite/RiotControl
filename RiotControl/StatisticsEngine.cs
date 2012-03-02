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
		NpgsqlConnection Database;
		List<RegionHandler> RegionHandlers;

		public StatisticsEngine(Configuration configuration)
		{
			EngineConfiguration = configuration;
			TerminationEvent = new AutoResetEvent(false);

			DatabaseConfiguration databaseConfiguration = configuration.Database;
			Database = new NpgsqlConnection("Server=" + databaseConfiguration.Host + ";Port=" + databaseConfiguration.Port + ";User Id=" + databaseConfiguration.Username + ";Database=" + databaseConfiguration.Database + ";");
		}

		public void RunEngine()
		{
			try
			{
				Database.Open();
			}
			catch (Exception exception)
			{
				Console.WriteLine("Unable to connect to SQL server: " + exception);
				return;
			}
			CreateRegionHandlers();
			TerminationEvent.WaitOne();
			Database.Close();
		}

		void CreateRegionHandlers()
		{
			RegionHandlers = new List<RegionHandler>();
			foreach (var profile in EngineConfiguration.RegionProfiles)
			{
				RegionHandler handler = new RegionHandler(EngineConfiguration, profile, Database);
				RegionHandlers.Add(handler);
			}
		}
	}
}
