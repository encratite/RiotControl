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

		public Program()
		{
			Serialiser = new Nil.Serialiser<Configuration>(ConfigurationPath);
			Configuration = Serialiser.Load();
			//Store it right away to automatically remove unused content and provide new default values
			Serialiser.Store(Configuration);

			Database databaseProvider = new Database(Configuration);
			StatisticsService = new StatisticsService(this, Configuration, databaseProvider);
			WebService = new WebService(this, Configuration, StatisticsService, databaseProvider);

			MainWindow = new MainWindow(Configuration, this, StatisticsService);
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
			//Nil.Output.WriteLine(line, arguments);
			MainWindow.WriteLine(line, arguments);
		}

		public void HandleException(Exception exception)
		{
			DumpAndTerminate(exception);
		}

		public void Terminate()
		{
			WebService.Terminate();
			StatisticsService.Terminate();
		}

		public static void DumpAndTerminate(Exception exception)
		{
			string threadName = Thread.CurrentThread.Name;
			if (threadName == null || threadName.Length == 0)
				threadName = "Main thread";
			string message = string.Format("[{0}] [r{1}] An exception of type {2} occurred in thread \"{3}\":\n{4}\n{5}\n\n", Nil.Time.Timestamp(), Assembly.GetEntryAssembly().GetName().Version.Revision, exception.GetType().ToString(), threadName, exception.Message, exception.StackTrace);
			Exception innerException = exception.InnerException;
			while (innerException != null)
			{
				message += string.Format("with an inner exception of type {0}:\n{1}\n{2}\n\n", innerException.GetType().ToString(), innerException.Message, innerException.StackTrace);
				innerException = innerException.InnerException;
			}
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
			try
			{
				Program program = new Program();
				program.Run();
			}
			catch (Exception exception)
			{
				DumpAndTerminate(exception);
			}
		}
	}
}
