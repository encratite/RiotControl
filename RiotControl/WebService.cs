using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Web.Script.Serialization;

using Blighttp;

using Nil;

namespace RiotControl
{
	partial class WebService
	{
		const string ProjectTitle = "Riot Control";

		StatisticsService Statistics;
		WebServer Server;

		Configuration ProgramConfiguration;
		WebConfiguration ServiceConfiguration;

		Database DatabaseProvider;

		Profiler WebServiceProfiler;

		JavaScriptSerializer Serialiser;

		HashSet<string> Views;
		Random PRNG;

		string IndexContents;

		public WebService(Configuration configuration, StatisticsService statisticsService, Database databaseProvider)
		{
			ProgramConfiguration = configuration;
			ServiceConfiguration = configuration.Web;
			Statistics = statisticsService;
			Server = new WebServer(ServiceConfiguration.Host, ServiceConfiguration.Port, Observe);

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
			WriteLine("Running web server on {0}:{1}", ServiceConfiguration.Host, ServiceConfiguration.Port);
			Server.Run();
		}

		void Observe(Request request)
		{
			WriteLine("[{0}] {1}", request.ClientAddress, request.Path);
		}

		void WriteLine(string message, params object[] arguments)
		{
			Output.WriteLine(string.Format("{0} {1}", Time.Timestamp(), message), arguments);
		}

		Document GetDocument(string title)
		{
			Document document = new Document(string.Format("{0} - {1}", title, ProjectTitle));
			document.Stylesheet = GetStaticPath("Style/Style.css");
			document.Icon = GetStaticPath("Icon/Icon.ico");
			return document;
		}

		string GetStaticPath(string path)
		{
			return string.Format("/Static/{0}", path);
		}

		string GetScript(string path)
		{
			string fullPath = GetStaticPath(string.Format("Script/{0}", path));
			return Markup.Script(fullPath);
		}

		string GetImage(string path)
		{
			return GetStaticPath(string.Format("Image/{0}", path));
		}

		string GetOverviewTable(Dictionary<string, string> fields)
		{
			string rows = "";
			foreach (var pair in fields)
				rows += Markup.TableRow(Markup.TableCell(Markup.Bold(pair.Key)) + Markup.TableCell(pair.Value));
			string overview = Markup.Table(rows, "summonerOverview");
			return overview;
		}

		string SignumString(int input)
		{
			if (input > 0)
				return Markup.Span(string.Format("+{0}", input), "positive").Trim();
			else if (input == 0)
				return string.Format("±{0}", input);
			else
				return Markup.Span(input.ToString(), "negative").Trim();
		}

		string Percentage(double input)
		{
			return string.Format("{0:0.0%}", input);
		}

		string Round(double input)
		{
			return string.Format("{0:0.0}", input);
		}

		string GetTableHeadRow(string[] titles)
		{
			string cells = "";
			foreach (var column in titles)
				cells += Markup.TableHead(column);
			string firstRow = Markup.TableRow(cells);
			return firstRow;
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
				return Statistics.GetWorkerByAbbreviation(abbreviation);
			}
			catch(Exception exception)
			{
				throw new HandlerException(exception.Message);
			}
		}

		Reply GetJSONRepy(object input)
		{
			string body = Serialiser.Serialize(input);
			Reply reply = new Reply(ReplyCode.Ok, ContentType.JSON, body);
			return reply;
		}

		DbConnection GetConnection()
		{
			return DatabaseProvider.GetConnection();
		}
	}
}
