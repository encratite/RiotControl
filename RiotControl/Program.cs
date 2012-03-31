using System;
using System.Windows;

namespace RiotControl
{
	class Program
	{
		static string ConfigurationPath = "Configuration.xml";

		Configuration Configuration;

		MainWindow MainWindow;

		StatisticsService StatisticsService;
		WebService WebService;

		public Program(Configuration configuration)
		{
			Configuration = configuration;

			MainWindow = new MainWindow();

			Database databaseProvider = new Database(configuration.Database);
			StatisticsService = new StatisticsService(this, configuration, databaseProvider);
			WebService = new WebService(this, configuration, StatisticsService, databaseProvider);
		}

		public void Run()
		{
			StatisticsService.Run();
			WebService.Run();
			MainWindow.ShowDialog();
		}

		public void WriteLine(string line, params object[] arguments)
		{
			string message = string.Format(line, arguments);
			MainWindow.AppendText(message);
		}

		[STAThread]
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
				MessageBox.Show(exception.Message);
				return;
			}

			Program program = new Program(configuration);
			program.Run();
		}
	}
}
