using System.Collections.Generic;
using System.Web.Script.Serialization;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		const string ProjectTitle = "Riot Control";

		const string SummonerFieldName = "summoner";

		StatisticsService Statistics;
		WebServer Server;

		Configuration ProgramConfiguration;
		WebConfiguration ServiceConfiguration;

		DatabaseConnectionProvider DatabaseProvider;

		Profiler WebServiceProfiler;

		JavaScriptSerializer Serialiser;

		Dictionary<int, string> ChampionNames;
		Dictionary<int, ItemInformation> Items;

		public WebService(Configuration configuration, StatisticsService statisticsService, DatabaseConnectionProvider databaseProvider)
		{
			ProgramConfiguration = configuration;
			ServiceConfiguration = configuration.Web;
			Statistics = statisticsService;
			Server = new WebServer(ServiceConfiguration.Host, ServiceConfiguration.Port);

			DatabaseProvider = databaseProvider;

			WebServiceProfiler = new Profiler();

			Serialiser = new JavaScriptSerializer();

			LoadChampionNames();
			LoadItemInformation();

			InitialiseHandlers();
		}

		public void Run()
		{
			Server.Run();
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
			return string.Format("/{0}/Static/{1}", ServiceConfiguration.Root, path);
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

		RegionHandler GetRegionHandler(string regionName)
		{
			RegionHandler regionHandler = Statistics.GetRegionHandler(regionName);
			if (regionHandler == null)
				throw new HandlerException("Invalid region handler");
			return regionHandler;
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

		string GetChampionName(int championId)
		{
			string name;
			if (ChampionNames.TryGetValue(championId, out name))
				return name;
			else
				return string.Format("Champion {0}", championId);
		}

		ItemInformation GetItemInformation(int itemId)
		{
			ItemInformation item;
			if (Items.TryGetValue(itemId, out item))
				return item;
			else
				return new ItemInformation(itemId, string.Format("Item {0}", itemId), "Unknown item");
		}

		string GetJavaScriptString(string input)
		{
			input = input.Replace("\\", "\\\\");
			input = input.Replace("'", "\\'");
			return string.Format("'{0}'", input);
		}
	}
}
