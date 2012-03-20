using System.Collections.Generic;

using Blighttp;

using Npgsql;

namespace RiotControl
{
	partial class WebService
	{
		Handler IndexHandler;
		Handler SearchHandler;
		Handler ViewSummonerHandler;
		Handler ViewSummonerGamesHandler;

		Handler LoadAccountDataHandler;
		Handler AutomaticUpdatesHandler;

		const string SummonerField = "summoner";
		const string RegionField = "region";

		void InitialiseHandlers()
		{
			IndexHandler = new Handler(Index);
			Server.Add(IndexHandler);

			SearchHandler = new Handler("Search", Search);
			Server.Add(SearchHandler);

			ViewSummonerGamesHandler = new Handler("SummonerGames", ViewSummonerGames, ArgumentType.String, ArgumentType.Integer);
			Server.Add(ViewSummonerGamesHandler);

			ViewSummonerHandler = new Handler("Summoner", ViewSummoner, ArgumentType.String, ArgumentType.Integer);
			Server.Add(ViewSummonerHandler);

			//JSON handlers

			LoadAccountDataHandler = new Handler("LoadAccountData", LoadAccountData, ArgumentType.String, ArgumentType.Integer);
			Server.Add(LoadAccountDataHandler);

			AutomaticUpdatesHandler = new Handler("AutomaticUpdates", AutomaticUpdates, ArgumentType.String, ArgumentType.Integer);
			Server.Add(AutomaticUpdatesHandler);
		}

		Reply Template(string title, string content, bool useSearchForm = true)
		{
			Document document = GetDocument(title);

			string logo = Markup.Image(GetImage("Logo.jpg"), ProjectTitle, id: "logo");

			if (useSearchForm)
			{
				string formBody = Markup.Text(SummonerField, null, "text");
				formBody += GetServerSelection();
				formBody += Markup.Submit("Search", "submit");
				string searchForm = Markup.Form(SearchHandler.GetPath(), formBody, id: "searchForm");

				content = searchForm + content;
			}

			string contentContainer = Markup.Diverse(content, id: "content");

			string body = logo + contentContainer;

			string output = document.Render(body);
			Reply reply = new Reply(output);
			return reply;
		}

		string GetServerSelection(string chosenRegion = null)
		{
			string options = "";
			foreach (var profile in Statistics.GetRegionProfiles())
				options += Markup.Option(profile.Abbreviation, profile.Description, profile.Abbreviation == chosenRegion);
			string select = Markup.Select(RegionField, options);
			return select;
		}

		Reply Index(Request request)
		{
			string title = "Index";
			string body = GetSearchForm("Enter the name of the summoner you want to look up:");
			return Template(title, body, false);
		}

		string GetSearchForm(string description, string summoner = null, string chosenRegion = null)
		{
			string formBody = Markup.Paragraph(description);
			formBody += Markup.Text(SummonerField, summoner, "text");
			formBody += GetServerSelection(chosenRegion);
			formBody += Markup.Submit("Search", "submit");
			string body = Markup.Form(SearchHandler.GetPath(), formBody, id: "indexForm");
			return body;
		}

		Reply Search(Request request)
		{
			var arguments = request.Content;
			string summonerName;
			if (!arguments.TryGetValue(SummonerField, out summonerName))
				throw new HandlerException("No summoner specified");
			string regionName;
			if (!arguments.TryGetValue(RegionField, out regionName))
				throw new HandlerException("No region specified");
			RegionHandler regionHandler = GetRegionHandler(regionName);

			using (NpgsqlConnection database = DatabaseProvider.GetConnection())
			{
				DatabaseCommand select = GetCommand("select account_id from summoner where lower(summoner_name) = lower(:summoner_name) and region = cast(:region as region_type)", database);
				select.Set("summoner_name", summonerName);
				select.SetEnum("region", regionHandler.GetRegionEnum());
				using (NpgsqlDataReader reader = select.ExecuteReader())
				{
					if (reader.Read())
					{
						int accountId = (int)reader[0];
						return Reply.Referral(ViewSummonerHandler.GetPath(regionName, accountId.ToString()));
					}
					else
					{
						LookupJob job = regionHandler.PerformSummonerLookup(summonerName);
						switch (job.Result)
						{
							case JobQueryResult.Success:
								return Reply.Referral(ViewSummonerHandler.GetPath(regionName, job.AccountId.ToString()));

							case JobQueryResult.NotFound:
								return Template("Search", GetSearchForm("Unable to find summoner.", summonerName, regionName), false);

							case JobQueryResult.Timeout:
								return Template("Search", GetSearchForm("A timeout has occurred.", summonerName, regionName), false);

							default:
								throw new HandlerException("Unknown job result");
						}
					}
				}
			}
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

		Reply ViewSummonerGames(Request request)
		{
			var arguments = request.Arguments;
			string regionName = (string)arguments[0];
			int accountId = (int)arguments[1];

			using (NpgsqlConnection database = DatabaseProvider.GetConnection())
			{
				Summoner summoner = LoadSummoner(regionName, accountId, database);
				List<GameTeamPlayer> games = LoadSummonerGames(summoner, database);
				string title = string.Format("Games of {0}", summoner.SummonerName);
				string table = GetSummonerGamesTable(summoner, games);
				Document document = GetDocument(title);
				Reply reply = new Reply(document.Render(table));
				return reply;
			}
		}

		Reply LoadAccountData(Request request)
		{
			var arguments = request.Arguments;
			string regionName = (string)arguments[0];
			int accountId = (int)arguments[1];
			RegionHandler regionHandler = GetRegionHandler(regionName);
			AccountIdJob job = regionHandler.PerformManualSummonerUpdate(accountId);
			SummonerUpdateResult result = new SummonerUpdateResult(job);
			string body = Serialiser.Serialize(result);
			Reply reply = new Reply(ReplyCode.Ok, ContentType.JSON, body);
			return reply;
		}

		Reply AutomaticUpdates(Request request)
		{
			var arguments = request.Arguments;
			string regionName = (string)arguments[0];
			int accountId = (int)arguments[1];
			RegionHandler regionHandler = GetRegionHandler(regionName);
			using (NpgsqlConnection database = DatabaseProvider.GetConnection())
			{
				DatabaseCommand command = GetCommand("update summoner set update_automatically = true where region = cast(:region as region_type) and account_id = :account_id", database);
				command.SetEnum("region", regionHandler.GetRegionEnum());
				command.Set("account_id", accountId);
				int rowsAffected = command.Execute();
				bool success = rowsAffected > 0;
				AutomaticUpdatesResult result = new AutomaticUpdatesResult(success);
				string body = Serialiser.Serialize(result);
				Reply reply = new Reply(ReplyCode.Ok, ContentType.JSON, body);
				return reply;
			}
		}
	}
}
