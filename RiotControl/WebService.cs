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

		JavaScriptSerializer Serialiser;

		Handler IndexHandler;
		Handler SearchHandler;
		Handler PerformSearchHandler;

		public WebService(WebConfiguration configuration, StatisticsService statisticsService)
		{
			Statistics = statisticsService;
			Server = new WebServer(configuration.Host, configuration.Port);

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

			List<ArgumentType> arguments = new List<ArgumentType>() { ArgumentType.String, ArgumentType.String };
			PerformSearchHandler = new Handler("PerformSearch", PerformSearch, arguments);
			rootContainer.Add(PerformSearchHandler);
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
			if (!arguments.ContainsKey(SummonerFieldName))
				throw new HandlerException("No summoner specified");
			string summoner = arguments[SummonerFieldName];
			string title = "Search results";
			string body = summoner;
			return GetReply(title, body);
		}

		Reply PerformSearch(Request request)
		{
			var arguments = request.Arguments;
			string regionName = (string)arguments[0];
			string summonerName = (string)arguments[1];
			RegionHandler regionHandler = Statistics.GetRegionHandler(regionName);
			if (regionHandler == null)
				throw new HandlerException("Invalid region handler");
			LookupJob job = regionHandler.PerformSummonerLookup(summonerName);
			SummonerSearchResult result = new SummonerSearchResult(job);
			string body = Serialiser.Serialize(result);
			Reply reply = new Reply(ReplyCode.Ok, ContentType.JSON, body);
			return reply;
		}
	}
}
