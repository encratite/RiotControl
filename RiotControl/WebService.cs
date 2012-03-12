using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

using Blighttp;

namespace RiotControl
{
	class WebService
	{
		const string ProjectTitle = "Riot Control";

		const string SummonerFieldName = "summoner";

		StatisticsService Statistics;
		WebServer Server;

		DatabaseConnectionProvider DatabaseProvider;

		JavaScriptSerializer Serialiser;

		Handler IndexHandler;
		Handler SearchHandler;
		Handler PerformSearchHandler;
		Handler ViewSummonerHandler;

		public WebService(WebConfiguration configuration, StatisticsService statisticsService, DatabaseConnectionProvider databaseProvider)
		{
			Statistics = statisticsService;
			Server = new WebServer(configuration.Host, configuration.Port);

			DatabaseProvider = databaseProvider;

			Serialiser = new JavaScriptSerializer();

			InitialiseHandlers(configuration.Root);
		}

		public void Run()
		{
			Server.Run();
		}

		void InitialiseHandlers(string root)
		{
			Handler rootContainer = new Handler(root);
			Server.Add(rootContainer);

			IndexHandler = new Handler(Index);
			rootContainer.Add(IndexHandler);

			SearchHandler = new Handler("Search", Search);
			rootContainer.Add(SearchHandler);

			PerformSearchHandler = new Handler("PerformSearch", PerformSearch, ArgumentType.String, ArgumentType.String);
			rootContainer.Add(PerformSearchHandler);

			ViewSummonerHandler = new Handler("Summoner", ViewSummoner, ArgumentType.String, ArgumentType.Integer);
			rootContainer.Add(ViewSummonerHandler);
		}

		Document GetDocument(string title)
		{
			Document document = new Document(string.Format("{0} - {1}", ProjectTitle, title));
			return document;
		}

		Reply GetReply(string title, string body)
		{
			Document document = GetDocument(title);
			string content = document.Render(body);
			Reply reply = new Reply(content);
			return reply;
		}

		Reply Index(Request request)
		{
			string title = "Index";
			string formBody = Markup.Paragraph("Enter the name of the summoner you want to look up:");
			formBody += Markup.Text(SummonerFieldName);
			formBody += Markup.Submit("Search");
			string path = SearchHandler.GetPath();
			string body = Markup.Form(path, formBody);
			return GetReply(title, body);
		}

		Reply Search(Request request)
		{
			var arguments = request.Content;
			string summoner;
			if (!arguments.TryGetValue(SummonerFieldName, out summoner))
				throw new HandlerException("No summoner specified");
			string title = "Search results";
			string body = summoner;
			return GetReply(title, body);
		}

		Reply PerformSearch(Request request)
		{
			var arguments = request.Arguments;
			string regionName = (string)arguments[0];
			string summonerName = (string)arguments[1];
			RegionHandler regionHandler = GetRegionHandler(regionName);
			LookupJob job = regionHandler.PerformSummonerLookup(summonerName);
			SummonerSearchResult result = new SummonerSearchResult(job);
			string body = Serialiser.Serialize(result);
			Reply reply = new Reply(ReplyCode.Ok, ContentType.JSON, body);
			return reply;
		}

		RegionHandler GetRegionHandler(string regionName)
		{
			RegionHandler regionHandler = Statistics.GetRegionHandler(regionName);
			if (regionHandler == null)
				throw new HandlerException("Invalid region handler");
			return regionHandler;
		}

		Reply ViewSummoner(Request request)
		{
			var arguments = request.Arguments;
			string regionName = (string)arguments[0];
			int accountId = (int)arguments[1];
			RegionHandler regionHandler = GetRegionHandler(regionName);
			throw new Exception("Not implemented");
		}
	}
}
