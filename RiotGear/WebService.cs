using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Web.Script.Serialization;

using Blighttp;

using Nil;

namespace RiotGear
{
	public partial class WebService
	{
		const string ProjectTitle = "Riot Control";

		IGlobalHandler GlobalHandler;

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

		public WebService(IGlobalHandler globalHandler, Configuration configuration, StatisticsService statisticsService, Database databaseProvider)
		{
			GlobalHandler = globalHandler;
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
			thread.Name = "Web service";
			thread.Start();
		}

		string GetServerString()
		{
			string host = ServiceConfiguration.Host;
			if (host == null || host.Length == 0)
				return string.Format("all addresses, port {0}", ServiceConfiguration.Port);
			else
				return string.Format("{0}:{1}", host, ServiceConfiguration.Port);
		}

		public void RunServer()
		{
			try
			{
				WriteLine("Running web server on {0}", GetServerString());
				Server.Run();
			}
			catch (SocketException exception)
			{
				WriteLine("Unable to run web server on {0}: {1}", GetServerString(), exception.Message);
				WriteLine("It is possible that port {0} is already being used. Make sure that no other instances of this application are currently running. It is also possible that another service is already using this port. In that case you will have to edit the Configuration.xml file and modify the <Port>{0}</Port> line to choose another port. You have to restart this application for any of the changes to take effect.", ServiceConfiguration.Port);
			}
			catch (Exception exception)
			{
				GlobalHandler.HandleException(exception);
			}
		}

		void Observe(Request request)
		{
			WriteLine("[HTTP {0}] {1}", request.ClientAddress, request.Path);
		}

		void WriteLine(string message, params object[] arguments)
		{
			GlobalHandler.WriteLine(message, arguments);
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

		public void Terminate()
		{
			Server.Terminate();
		}
	}
}
