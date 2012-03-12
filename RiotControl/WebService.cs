using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Blighttp;

namespace RiotControl
{
	class WebService
	{
		const string ProjectTitle = "Riot Control";

		const string SummonerFieldName = "summoner";

		StatisticsService Statistics;
		WebServer Server;

		Handler IndexHandler;
		Handler SearchHandler;

		public WebService(WebConfiguration configuration, StatisticsService statisticsService)
		{
			Statistics = statisticsService;
			Server = new WebServer(configuration.Host, configuration.Port);

			InitialiseHandlers(configuration.Root);
		}

		void InitialiseHandlers(string root)
		{
			Handler rootContainer = new Handler(root);
			Server.Add(rootContainer);

			IndexHandler = new Handler(Index);
			rootContainer.Add(IndexHandler);

			SearchHandler = new Handler("Search", Search);
			rootContainer.Add(SearchHandler);
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

		public void Run()
		{
			Server.Run();
		}
	}
}
