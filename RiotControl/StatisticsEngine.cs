using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Npgsql;

namespace RiotControl
{
	class StatisticsEngine
	{
		Configuration EngineConfiguration;

		NpgsqlConnection Database;

		List<RegionHandler> RegionHandlers;

		public StatisticsEngine(Configuration configuration)
		{
			EngineConfiguration = configuration;

			DatabaseConfiguration databaseConfiguration = configuration.Database;
			Database = new NpgsqlConnection("Server=" + databaseConfiguration.Host + ";Port=" + databaseConfiguration.Port + ";User Id=" + databaseConfiguration.Username + ";Database=" + databaseConfiguration.Database + ";");
		}

		public bool RunEngine()
		{
			try
			{
				Database.Open();
			}
			catch (Exception exception)
			{
				Console.WriteLine("Unable to connect to SQL server: " + exception);
				return false;
			}
			CreateRegionHandlers();
			return true;
		}

		void CreateRegionHandlers()
		{
			foreach (var profile in EngineConfiguration.RegionProfiles)
			{
				RegionHandler handler = new RegionHandler(profile, Database);
				RegionHandlers.Add(handler);
			}
		}
	}
}
