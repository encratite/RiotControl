using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

using RiotGear;

namespace RiotControl
{
	public class Program : IGlobalHandler
	{
		const string ConfigurationPath = "Configuration.xml";
		const string ErrorFilePath = "Error.txt";

		Nil.Serialiser<Configuration> Serialiser;
		Configuration Configuration;

		MainWindow MainWindow;

		StatisticsService StatisticsService;
		WebService WebService;

		public bool Initialise()
		{
			try
			{
				Serialiser = new Nil.Serialiser<Configuration>(ConfigurationPath);
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

		//Interface implementation

		public void WriteLine(string line, params object[] arguments)
		{
			string message = string.Format(line, arguments);
			MainWindow.WriteLine(message);
		}

		public void HandleException(Exception exception)
		{
			DumpAndTerminate(exception);
		}

		public static void DumpAndTerminate(Exception exception)
		{
			string message = string.Format("[{0}] [r{1}] An exception of type {2} occurred in thread {3}:\n{4}\n{5}\n\n", Nil.Time.Timestamp(), Assembly.GetEntryAssembly().GetName().Version.Revision, exception.GetType().ToString(), Thread.CurrentThread.Name, exception.Message, exception.StackTrace);
			//Make the dump easier to read with Notepad by using \r\n line endings instead of \n ones
			message = message.Replace("\r", "");
			message = message.Replace("\n", "\r\n");
			using (StreamWriter writer = File.AppendText(ErrorFilePath))
			{
				writer.Write(message);
				writer.Close();
			}
			MessageBox.Show(string.Format("An exception of type {0} occurred. An error log file ({1}) has been created. The application will now terminate.", exception.GetType().ToString(), ErrorFilePath), "Error");
			Environment.Exit(1);
		}

		[STAThread]
		static void Main(string[] arguments)
		{
			Program program = new Program();
			if (!program.Initialise())
				return;
			try
			{
				program.Run();
			}
			catch (Exception exception)
			{
				DumpAndTerminate(exception);
			}
		}
	}
}
