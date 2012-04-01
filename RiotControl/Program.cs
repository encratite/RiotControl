using System;
using System.Windows;

using Nil;

namespace RiotControl
{
	public class Program
	{
		const string ConfigurationPath = "Configuration.xml";

		Serialiser<Configuration> Serialiser;
		Configuration Configuration;

		MainWindow MainWindow;

		StatisticsService StatisticsService;
		WebService WebService;

		public bool Initialise()
		{
			try
			{
				Serialiser = new Serialiser<Configuration>(ConfigurationPath);
				Configuration = Serialiser.Load();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
				return false;
			}

			Database databaseProvider = new Database(Configuration.Database);
			StatisticsService = new StatisticsService(this, Configuration, databaseProvider);
			WebService = new WebService(this, Configuration, StatisticsService, databaseProvider);

			MainWindow = new MainWindow(Configuration, this, StatisticsService);

			return true;
		}

		public void SaveConfiguration()
		{
			Serialiser.Store(Configuration);
		}

		public void Run()
		{
			WebService.Run();
			StatisticsService.Run();
			MainWindow.ShowDialog();
		}

		public void WriteLine(string line, params object[] arguments)
		{
			string message = string.Format(line, arguments);
			MainWindow.WriteLine(message);
		}

		[STAThread]
		static void Main(string[] arguments)
		{
			Program program = new Program();
			if (!program.Initialise())
				return;
			program.Run();
		}
	}
}
