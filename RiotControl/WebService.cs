using System;
using System.Collections.Generic;
using System.Linq;
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

		Dictionary<int, string> ChampionNames;

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

			LoadChampionNames();

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

		string GetOverviewTable(Dictionary<string, string> fields)
		{
			string rows = "";
			foreach (var pair in fields)
				rows += Markup.TableRow(Markup.TableCell(Markup.Span(pair.Key)) + Markup.TableCell(Markup.Escape(pair.Value)));
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

		string GetSummonerOverview(string regionName, Summoner summoner)
		{
			string profileIcon = Markup.Image(GetImage(string.Format("Profile/profileIcon{0}.jpg", summoner.ProfileIcon)), string.Format("{0}'s profile icon", summoner.SummonerName), id: "profileIcon");

			var overviewFields1 = new Dictionary<string, string>()
			{
				{"Summoner name", summoner.SummonerName},
				{"Internal name", summoner.InternalName},
				{"Region", regionName},
				{"Summoner level", summoner.SummonerLevel.ToString()},
				{"Account ID", summoner.AccountId.ToString()},
				{"Summoner ID", summoner.SummonerId.ToString()},
				{"Non-custom games played", summoner.GetGamesPlayed().ToString()},
			};

			var overviewFields2 = new Dictionary<string, string>()
			{
				{"First update", summoner.TimeCreated.ToString()},
				{"Last update", summoner.TimeUpdated.ToString()},
				{"Is updated automatically", summoner.UpdateAutomatically ? "Yes" : "No"},
			};

			string overview = Markup.Diverse(profileIcon + GetOverviewTable(overviewFields1) + GetOverviewTable(overviewFields2), id: "summonerHeader");

			return overview;
		}

		string GetTableHeadRow(string[] titles)
		{
			string cells = "";
			foreach (var column in titles)
				cells += Markup.TableHead(column);
			string firstRow = Markup.TableRow(cells);
			return firstRow;
		}

		string GetRatingTable(Summoner summoner)
		{
			string[] columnTitles =
			{
				"Map",
				"Mode",
				"Games",
				"W",
				"L",
				"W - L",
				"WR",
				"Left",
				"Rating",
				"Top rating",
			};

			string caption = Markup.Caption("General Statistics");

			string firstRow = GetTableHeadRow(columnTitles);

			int rowCount = 0;
			string otherRows = "";
			foreach (var rating in summoner.Ratings)
			{
				int gamesPlayed = rating.Wins + rating.Losses;
				if (gamesPlayed == 0)
					continue;
				string row = "";
				row += Markup.TableCell(rating.Map.GetString());
				row += Markup.TableCell(rating.GameMode.GetString());
				row += Markup.TableCell(gamesPlayed.ToString());
				row += Markup.TableCell(rating.Wins.ToString());
				row += Markup.TableCell(rating.Losses.ToString());
				row += Markup.TableCell(SignumString(rating.Wins - rating.Losses));
				row += Markup.TableCell(Percentage(((double)rating.Wins) / (rating.Wins + rating.Losses)));
				row += Markup.TableCell(rating.Leaves.ToString());
				if (rating.CurrentRating == null)
					row += Markup.TableCell("?");
				else
					row += Markup.TableCell(rating.CurrentRating.ToString());
				if (rating.CurrentRating == null)
					row += Markup.TableCell("?");
				else
					row += Markup.TableCell(string.Format("{0} ({1})", rating.TopRating, SignumString(rating.TopRating.Value - rating.CurrentRating.Value)));
				otherRows += Markup.TableRow(row);
				rowCount++;
			}
			if (rowCount > 0)
			{
				string ratingTable = Markup.Table(caption + firstRow + otherRows);
				return ratingTable;
			}
			else
				return "";
		}

		string GetRankedStatistics(Summoner summoner)
		{
			string table = Markup.Diverse("", id: "rankedStatistics");
			string statisticsScript = GetScript("Statistics.js");

			string inline = "var rankedStatistics =\n[\n";
			foreach (var champion in summoner.RankedStatistics)
			{
				int[] fields =
				{
					champion.Wins,
					champion.Losses,

					champion.Kills,
					champion.Deaths,
					champion.Assists,

					champion.MinionKills,

					champion.Gold,

					champion.TurretsDestroyed,

					champion.DamageDealt,
					champion.PhysicalDamageDealt,
					champion.MagicalDamageDealt,

					champion.DamageTaken,

					champion.DoubleKills,
					champion.TripleKills,
					champion.QuadraKills,
					champion.PentaKills,

					champion.TimeSpentDead,

					champion.MaximumKills,
					champion.MaximumDeaths,
				};
				var stringFields = from x in fields select x.ToString();
				inline += "new RankedStatistics(";
				inline += GetJavaScriptString(champion.ChampionName) + ", ";
				inline += string.Join(", ", stringFields);
				inline += "),\n";
			}
			inline += "];\n";
			inline += "writeTable(rankedStatistics);";

			string inlineScript = Markup.InlineScript(inline);

			string output = table + statisticsScript + inlineScript;

			return output;
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
				LoadSummonerRankedStatistics(summoner, database);

				string title = summoner.SummonerName;

				string overview = GetSummonerOverview(regionName, summoner);
				string ratingTable = GetRatingTable(summoner);
				string rankedStatisticsTable = GetRankedStatistics(summoner);

				string body = overview + ratingTable + rankedStatisticsTable;
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

		void LoadSummonerRankedStatistics(Summoner summoner, NpgsqlConnection database)
		{
			SQLCommand select = GetCommand("select {0} from summoner_ranked_statistics where summoner_id = :summoner_id", database, SummonerRankedStatistics.GetFields());
			select.Set("summoner_id", summoner.Id);
			using (NpgsqlDataReader reader = select.ExecuteReader())
			{
				while (reader.Read())
				{
					SummonerRankedStatistics statistics = new SummonerRankedStatistics(reader);
					if (!ChampionNames.TryGetValue(statistics.ChampionId, out statistics.ChampionName))
						statistics.ChampionName = string.Format("Champion {0}", statistics.ChampionId);
					summoner.RankedStatistics.Add(statistics);
				}
				summoner.RankedStatistics.Sort();
			}
		}

		string GetJavaScriptString(string input)
		{
			input = input.Replace("\\", "\\\\");
			input = input.Replace("'", "\\'");
			return string.Format("'{0}'", input);
		}

		void LoadChampionNames()
		{
			ChampionNames = new Dictionary<int, string>();
			using (NpgsqlConnection database = DatabaseProvider.GetConnection())
			{
				SQLCommand select = GetCommand("select champion_id, champion_name from champion_name", database);
				using (NpgsqlDataReader reader = select.ExecuteReader())
				{
					while (reader.Read())
					{
						int championId = (int)reader[0];
						string championName = (string)reader[1];
						ChampionNames[championId] = championName;
					}
				}
			}
		}
	}
}
