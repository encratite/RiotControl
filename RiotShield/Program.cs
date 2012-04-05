using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading;

using RiotGear;

namespace RiotShield
{
	class Program : IGlobalHandler
	{
		const string ConfigurationPath = "Configuration.xml";
		const string ErrorFilePath = "Error.txt";

		Nil.Serialiser<Configuration> Serialiser;
		Configuration Configuration;

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
		}

		public void Run()
		{
			StatisticsService.Run();
			WebService.RunServer();
		}

		//Interface implementation

		public void WriteLine(string line, params object[] arguments)
		{
			Nil.Output.WriteLine(line, arguments);
		}

		public void HandleException(Exception exception)
		{
			DumpAndTerminate(exception);
		}

		public static void DumpAndTerminate(Exception exception)
		{
			string message = string.Format("[{0}] [r{1}] An exception of type {2} occurred in thread \"{3}\":\n{4}\n{5}\n\n", Nil.Time.Timestamp(), Assembly.GetEntryAssembly().GetName().Version.Revision, exception.GetType().ToString(), Thread.CurrentThread.Name, exception.Message, exception.StackTrace);
			using (StreamWriter writer = File.AppendText(ErrorFilePath))
			{
				writer.Write(message);
				writer.Close();
			}
			Nil.Output.WriteLine(message);
			Environment.Exit(1);
		}

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
