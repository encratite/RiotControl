using System;

namespace RiotControl
{
	class Program
	{
		static string ConfigurationPath = "Configuration.xml";

		static void Main(string[] arguments)
		{
			Configuration configuration;
			try
			{
				Nil.Serialiser<Configuration> serialiser = new Nil.Serialiser<Configuration>(ConfigurationPath);
				configuration = serialiser.Load();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				return;
			}

			Database databaseProvider = new Database(configuration.Database);

			StatisticsService statisticsService = new StatisticsService(configuration, databaseProvider);
			statisticsService.Run();

			WebService webService = new WebService(configuration, statisticsService, databaseProvider);
			webService.Run();
		}
	}
}
