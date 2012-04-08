using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
		UpdateService UpdateService;

		public Program()
		{
			Serialiser = new Nil.Serialiser<Configuration>(ConfigurationPath);
			Configuration = Serialiser.Load();
			//Check for configuration errors
			Configuration.Check();
			//Store it right away to automatically remove unused content and provide new default values
			Serialiser.Store(Configuration);

			Database databaseProvider = new Database(Configuration);
			StatisticsService = new StatisticsService(this, Configuration, databaseProvider);
			WebService = new WebService(this, Configuration, StatisticsService, databaseProvider);
			UpdateService = new UpdateService(Configuration, this);
		}

		public void Run()
		{
			UpdateService.Cleanup();
			if (Configuration.Updates.EnableAutomaticUpdates)
				UpdateService.CheckForUpdate();
			StatisticsService.Run();
			WebService.RunServer();
		}

#region IGlobalHandler interface

		public void WriteLine(string line, params object[] arguments)
		{
			Nil.Output.WriteLine(line, arguments);
		}

		public void HandleException(Exception exception)
		{
			DumpAndTerminate(exception);
		}
#endregion

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
			using (StreamWriter writer = File.AppendText(ErrorFilePath))
			{
				writer.Write(message);
				writer.Close();
			}
			Console.Write(message);
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
