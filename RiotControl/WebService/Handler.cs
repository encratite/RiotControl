using Blighttp;

using Npgsql;

namespace RiotControl
{
	partial class WebService
	{
		Handler IndexHandler;
		Handler SearchHandler;
		Handler ViewSummonerHandler;

		Handler PerformSearchHandler;
		Handler UpdateSummonerHandler;

		void InitialiseHandlers()
		{
			Handler rootContainer = new Handler(ServiceConfiguration.Root);
			Server.Add(rootContainer);

			IndexHandler = new Handler(Index);
			rootContainer.Add(IndexHandler);

			SearchHandler = new Handler("Search", Search);
			rootContainer.Add(SearchHandler);

			ViewSummonerHandler = new Handler("Summoner", ViewSummoner, ArgumentType.String, ArgumentType.Integer);
			rootContainer.Add(ViewSummonerHandler);

			PerformSearchHandler = new Handler("FindSummoner", FindSummoner, ArgumentType.String, ArgumentType.String);
			rootContainer.Add(PerformSearchHandler);

			UpdateSummonerHandler = new Handler("UpdateSummoner", UpdateSummoner, ArgumentType.String, ArgumentType.Integer);
			rootContainer.Add(UpdateSummonerHandler);
		}

		Reply Template(string title, string content, bool useSearchForm = true)
		{
			Document document = GetDocument(title);
			document.Stylesheet = GetStaticPath("Style/Style.css");
			document.Icon = GetStaticPath("Icon/Icon.ico");

			string logo = Markup.Image(GetImage("Logo.jpg"), ProjectTitle, id: "logo");

			if (useSearchForm)
			{
				string formBody = Markup.Text(SummonerFieldName, null, "text");
				formBody += Markup.Submit("Search", "submit");
				string path = SearchHandler.GetPath();
				string searchForm = Markup.Form(path, formBody, id: "searchForm");

				content = searchForm + content;
			}

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
			return Template(title, body, false);
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
				LoadAggregatedChampionStatistics(summoner, database);

				string title = summoner.SummonerName;

				string script = GetScript("Statistics.js");
				string overview = GetSummonerOverview(regionName, summoner);
				string rating = GetRatingTable(summoner);
				string rankedStatistics = GetRankedStatistics(summoner);
				string aggregatedStatistics = GetAggregatedChampionStatistics(summoner);

				string body = script + overview + rating + rankedStatistics + aggregatedStatistics;
				return Template(title, body);
			}
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

		Reply UpdateSummoner(Request request)
		{
			var arguments = request.Arguments;
			string regionName = (string)arguments[0];
			int accountId = (int)arguments[1];
			RegionHandler regionHandler = GetRegionHandler(regionName);
			UpdateJob job = regionHandler.PerformManualSummonerUpdate(accountId);
			SummonerUpdateResult result = new SummonerUpdateResult(job);
			string body = Serialiser.Serialize(result);
			Reply reply = new Reply(ReplyCode.Ok, ContentType.JSON, body);
			return reply;
		}
	}
}
