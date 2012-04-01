using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;

using Blighttp;

using Nil;

namespace RiotControl
{
	partial class WebService
	{
		const string ProjectTitle = "Riot Control";

		Program Program;

		StatisticsService StatisticsService;
		WebServer Server;

		Configuration ProgramConfiguration;
		WebConfiguration ServiceConfiguration;

		Database DatabaseProvider;

		Profiler WebServiceProfiler;

		JavaScriptSerializer Serialiser;

		HashSet<string> Views;
		Random PRNG;

		string IndexContents;

		public WebService(Program program, Configuration configuration, StatisticsService statisticsService, Database databaseProvider)
		{
			Program = program;
			ProgramConfiguration = configuration;
			ServiceConfiguration = configuration.Web;
			StatisticsService = statisticsService;
			Server = new WebServer(ServiceConfiguration.Host, ServiceConfiguration.Port, Observe, ServiceConfiguration.EnableReverseProxyRealIPMode);

			DatabaseProvider = databaseProvider;

			WebServiceProfiler = new Profiler();

			Serialiser = new JavaScriptSerializer();

			Views = new HashSet<string>();
			PRNG = new Random();

			LoadIndex();
			InitialiseHandlers();
		}

		void LoadIndex()
		{
			try
			{
				IndexContents = System.IO.File.ReadAllText(ProgramConfiguration.Index);
			}
			catch (FileNotFoundException)
			{
				throw new Exception("Unable to read index file");
			}
		}

		public void Run()
		{
			Thread thread = new Thread(RunServer);
			thread.Name = "WebService";
			thread.Start();
		}

		void RunServer()
		{
			try
			{
				WriteLine("Running web server on {0}:{1}", ServiceConfiguration.Host, ServiceConfiguration.Port);
				Server.Run();
			}
			catch (Exception exception)
			{
				Program.DumpAndTerminate(exception);
			}
		}

		void Observe(Request request)
		{
			WriteLine("[HTTP {0}] {1}", request.ClientAddress, request.Path);
		}

		void WriteLine(string message, params object[] arguments)
		{
			Program.WriteLine(message, arguments);
		}

		string GetJavaScriptString(string input)
		{
			input = input.Replace("\\", "\\\\");
			input = input.Replace("'", "\\'");
			return string.Format("'{0}'", input);
		}

		Worker GetWorkerByAbbreviation(string abbreviation)
		{
			try
			{
				return StatisticsService.GetWorkerByAbbreviation(abbreviation);
			}
			catch(Exception exception)
			{
				throw new HandlerException(exception.Message);
			}
		}

		Reply GetJSONReply(object input)
		{
			string body = Serialiser.Serialize(input);
			Reply reply = new Reply(ReplyCode.Ok, ContentType.JSON, body);
			return reply;
		}

		DbConnection GetConnection()
		{
			return DatabaseProvider.GetConnection();
		}

		bool IsPrivileged(string address)
		{
			return ServiceConfiguration.PrivilegedAddresses.Contains(address);
		}

		void PrivilegeCheck(Request request)
		{
			if (!IsPrivileged(request.ClientAddress))
				throw new HandlerException("You do not have the permission to use this service");
		}
	}
}
