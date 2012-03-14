using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

using Npgsql;

using Blighttp;

namespace RiotControl
{
	class WebService
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

		Handler IndexHandler;
		Handler SearchHandler;
		Handler PerformSearchHandler;
		Handler ViewSummonerHandler;

		public WebService(Configuration configuration, StatisticsService statisticsService, DatabaseConnectionProvider databaseProvider)
		{
			ProgramConfiguration = configuration;
			ServiceConfiguration = configuration.Web;
			Statistics = statisticsService;
			Server = new WebServer(ServiceConfiguration.Host, ServiceConfiguration.Port);

			DatabaseProvider = databaseProvider;

			WebServiceProfiler = new Profiler();

			Serialiser = new JavaScriptSerializer();

			InitialiseHandlers();
		}

		public void Run()
		{
			Server.Run();
		}

		void InitialiseHandlers()
		{
			Handler rootContainer = new Handler(ServiceConfiguration.Root);
			Server.Add(rootContainer);

			IndexHandler = new Handler(Index);
			rootContainer.Add(IndexHandler);

			SearchHandler = new Handler("Search", Search);
			rootContainer.Add(SearchHandler);

			PerformSearchHandler = new Handler("FindSummoner", FindSummoner, ArgumentType.String, ArgumentType.String);
			rootContainer.Add(PerformSearchHandler);

			ViewSummonerHandler = new Handler("Summoner", ViewSummoner, ArgumentType.String, ArgumentType.Integer);
			rootContainer.Add(ViewSummonerHandler);
		}

		Document GetDocument(string title)
		{
			Document document = new Document(string.Format("{0} - {1}", title, ProjectTitle));
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

		Reply Template(string title, string content)
		{
			Document document = GetDocument(title);
			document.Stylesheet = GetStaticPath("Style/Style.css");
			document.Icon = GetStaticPath("Icon/Icon.ico");
			string logo = Markup.Image(GetImage("Logo.jpg"), ProjectTitle, id: "logo");
			string contentContainer = Markup.Diverse(content, id: "content");
			string body = logo + contentContainer;
			string output = document.Render(body);
			Reply reply = new Reply(output);
			return reply;
		}

		Reply Index(Request request)
		{
			string title = "Index";
			string formBody = Markup.Paragraph("Enter the name of the summoner you want to look up:");
			formBody += Markup.Text(SummonerFieldName, null, "text");
			formBody += Markup.Submit("Search", "submit");
			string path = SearchHandler.GetPath();
			string body = Markup.Form(path, formBody, id: "indexForm");
			return Template(title, body);
		}

		Reply Search(Request request)
		{
			var arguments = request.Content;
			string summoner;
			if (!arguments.TryGetValue(SummonerFieldName, out summoner))
				throw new HandlerException("No summoner specified");
			string title = string.Format("Search results for \"{0}\"", summoner);
			string rows = Markup.TableRow(Markup.TableHead("Region") + Markup.TableHead("Result") + Markup.TableHead("Duration"));
			foreach (var region in ProgramConfiguration.RegionProfiles)
			{
				string resultId = "Result" + region.Abbreviation;
				string durationId = "Duration" + region.Abbreviation;
				rows += Markup.TableRow(Markup.TableCell(region.Description) + Markup.TableCell("", id: resultId) + Markup.TableCell("", id: durationId));
			}
			string script = GetScript("Search.js") + Markup.InlineScript(string.Format("findSummoner({0});", GetJavaScriptString(summoner))); ;
			string body = Markup.Table(rows) + script;
			return Template(title, body);
		}

		Reply FindSummoner(Request request)
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

			using (NpgsqlConnection database = DatabaseProvider.GetConnection())
			{
				Summoner summoner = LoadSummoner(regionName, accountId, database);
				LoadSummonerRating(summoner, database);
				string title = summoner.SummonerName;
				string profileIcon = Markup.Image(GetImage(string.Format("Profile/profileIcon{0}.jpg", summoner.ProfileIcon)), string.Format("{0}'s profile icon", summoner.SummonerName), id: "profileIcon");
				string name = Markup.Paragraph(Markup.Escape(summoner.SummonerName));
				string level = Markup.Paragraph(string.Format("Level {0}", summoner.SummonerLevel));
				string description = Markup.Diverse(name + level, id: "summonerDescription");
				string head = Markup.Diverse(profileIcon + description, id: "summonerHeader");

				Dictionary<string, string> overviewFields = new Dictionary<string, string>()
				{
					{"Summoner name", summoner.SummonerName},
					{"Internal name", summoner.InternalName},
					{"Account ID", summoner.AccountId.ToString()},
					{"Summoner ID", summoner.SummonerId.ToString()},
					{"Summoner level", summoner.SummonerLevel.ToString()},
					{"First update", summoner.TimeCreated.ToString()},
					{"Last update", summoner.TimeUpdated.ToString()},
					{"Subscriber", summoner.UpdateAutomatically ? "Yes" : "No"},
				};

				string rows = "";
				foreach (var pair in overviewFields)
					rows += Markup.TableRow(Markup.TableCell(Markup.Span(pair.Key)) + Markup.TableCell(Markup.Escape(pair.Value)));
				string overview = Markup.Table(rows, id: "summonerOverview");

				string body = head + overview;
				return Template(title, body);
			}
		}

		SQLCommand GetCommand(string query, NpgsqlConnection database, params object[] arguments)
		{
			return new SQLCommand(query, database, WebServiceProfiler, arguments);
		}

		Summoner LoadSummoner(string regionName, int accountId, NpgsqlConnection database)
		{
			RegionHandler regionHandler = GetRegionHandler(regionName);
			SQLCommand select = GetCommand("select {0} from summoner where region = cast(:region as region_type) and account_id = :account_id", database, Summoner.GetFields());
			select.SetEnum("region", regionHandler.GetRegionEnum());
			select.Set("account_id", accountId);
			using (NpgsqlDataReader reader = select.ExecuteReader())
			{
				Summoner summoner = null;
				if (reader.Read())
					summoner = new Summoner(reader);
				if (summoner == null)
					throw new HandlerException("No such summoner");
				return summoner;
			}
		}

		int CompareRatings(SummonerRating x, SummonerRating y)
		{
			int output = x.Map.CompareTo(y.Map);
			if (output == 0)
				return x.GameMode.CompareTo(y.GameMode);
			else
				return output;
		}

		void LoadSummonerRating(Summoner summoner, NpgsqlConnection database)
		{
			SQLCommand select = GetCommand("select {0} from summoner_rating where summoner_id = :summoner_id", database, SummonerRating.GetFields());
			select.Set("summoner_id", summoner.Id);
			using (NpgsqlDataReader reader = select.ExecuteReader())
			{
				while (reader.Read())
				{
					SummonerRating rating = new SummonerRating(reader);
					summoner.Ratings.Add(rating);
					Dictionary<GameModeType, SummonerRating> dictionary;
					if (!summoner.RatingDictionary.TryGetValue(rating.Map, out dictionary))
					{
						dictionary = new Dictionary<GameModeType, SummonerRating>();
						summoner.RatingDictionary[rating.Map] = dictionary;
					}
					dictionary[rating.GameMode] = rating;
				}
				summoner.Ratings.Sort(CompareRatings);
			}
		}

		string GetJavaScriptString(string input)
		{
			input = input.Replace("\\", "\\\\");
			input = input.Replace("'", "\\'");
			return string.Format("'{0}'", input);
		}
	}
}
