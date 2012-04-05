using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

using Blighttp;

namespace RiotGear
{
	public partial class WebService
	{
		Handler ApiSearchHandler;
		Handler ApiUpdateHandler;
		Handler ApiSummonerProfileHandler;
		Handler ApiSummonerStatisticsHandler;
		Handler ApiSummonerGamesHandler;
		Handler ApiSummonerRunesHandler;
		Handler ApiSetAutomaticUpdatesHandler;

		Handler IndexHandler;

		void InitialiseHandlers()
		{
			Handler apiContainer = new Handler("API");
			Server.Add(apiContainer);

			ApiSearchHandler = new Handler("Search", ApiSearch, ArgumentType.String, ArgumentType.String);
			apiContainer.Add(ApiSearchHandler);

			ApiUpdateHandler = new Handler("Update", ApiUpdateSummoner, ArgumentType.String, ArgumentType.Integer);
			apiContainer.Add(ApiUpdateHandler);

			ApiSummonerProfileHandler = new Handler("Profile", ApiSummonerProfile, ArgumentType.String, ArgumentType.Integer);
			apiContainer.Add(ApiSummonerProfileHandler);

			ApiSummonerStatisticsHandler = new Handler("Statistics", ApiSummonerStatistics, ArgumentType.String, ArgumentType.Integer);
			apiContainer.Add(ApiSummonerStatisticsHandler);

			ApiSummonerGamesHandler = new Handler("Games", ApiSummonerGames, ArgumentType.String, ArgumentType.Integer);
			apiContainer.Add(ApiSummonerGamesHandler);

			ApiSummonerRunesHandler = new Handler("Runes", ApiSummonerRunes, ArgumentType.String, ArgumentType.Integer);
			apiContainer.Add(ApiSummonerRunesHandler);

			ApiSetAutomaticUpdatesHandler = new Handler("SetAutomaticUpdates", ApiSetAutomaticUpdates, ArgumentType.String, ArgumentType.Integer, ArgumentType.Integer);
			apiContainer.Add(ApiSetAutomaticUpdatesHandler);

			IndexHandler = new Handler(Index);
			Server.Add(IndexHandler);
		}

		Reply ApiSearch(Request request)
		{
			PrivilegeCheck(request);
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			string summonerName = (string)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			Summoner summoner = null;
			OperationResult result = worker.FindSummoner(summonerName, ref summoner);
			SummonerSearchResult output;
			if (result == OperationResult.Success)
				output = new SummonerSearchResult(summoner.AccountId);
			else
				output = new SummonerSearchResult(result);
			return GetJSONReply(output);
		}

		Reply ApiUpdateSummoner(Request request)
		{
			PrivilegeCheck(request);
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			OperationResult result = worker.UpdateSummonerByAccountId(accountId);
			SummonerUpdateResult output = new SummonerUpdateResult(result);
			return GetJSONReply(output);
		}

		Reply ApiSummonerProfile(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			SummonerProfileResult output;
			Summoner summoner = StatisticsService.GetSummoner(worker.Region, accountId);
			if (summoner != null)
				output = new SummonerProfileResult(summoner);
			else
				output = new SummonerProfileResult(OperationResult.NotFound);
			return GetJSONReply(output);
		}

		Reply ApiSummonerStatistics(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			SummonerStatisticsResult output;
			Summoner summoner = StatisticsService.GetSummoner(worker.Region, accountId);
			if (summoner != null)
			{
				using (var connection = GetConnection())
				{
					SummonerStatistics statistics = GetSummonerStatistics(summoner, connection);
					output = new SummonerStatisticsResult(statistics);
				}
			}
			else
				output = new SummonerStatisticsResult(OperationResult.NotFound);
			return GetJSONReply(output);
		}

		Reply ApiSummonerGames(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			SummonerGamesResult output;
			Summoner summoner = StatisticsService.GetSummoner(worker.Region, accountId);
			if (summoner != null)
			{
				using (var connection = GetConnection())
				{
					List<ExtendedPlayer> games = GetSummonerGames(summoner, connection);
					output = new SummonerGamesResult(games);
				}
			}
			else
				output = new SummonerGamesResult(OperationResult.NotFound);
			return GetJSONReply(output);
		}

		Reply ApiSummonerRunes(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			SummonerRunesResult output;
			Summoner summoner = StatisticsService.GetSummoner(worker.Region, accountId);
			if (summoner != null)
			{
				List<RunePage> runePages = GetRunePages(summoner);
				output = new SummonerRunesResult(runePages);
			}
			else
				output = new SummonerRunesResult(OperationResult.NotFound);
			return GetJSONReply(output);
		}

		Reply ApiSetAutomaticUpdates(Request request)
		{
			PrivilegeCheck(request);
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			bool updateAutomatically = (int)request.Arguments[2] != 0;
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			SummonerAutomaticUpdatesResult output;
			Summoner summoner = StatisticsService.GetSummoner(worker.Region, accountId);
			if (summoner != null)
			{
				OperationResult result = SetSummonerAutomaticUpdates(summoner, updateAutomatically);
				output = new SummonerAutomaticUpdatesResult(result);
			}
			else
				output = new SummonerAutomaticUpdatesResult(OperationResult.NotFound);
			return GetJSONReply(output);
		}

		Reply Index(Request request)
		{
			List<string> regionStrings = new List<string>();
			foreach (var profile in StatisticsService.GetActiveProfiles())
			{
				//Avoid race conditions since the profile is modified by other threads
				lock (profile)
					regionStrings.Add(string.Format("[{0}, {1}, {2}]", GetJavaScriptString(profile.Abbreviation), GetJavaScriptString(profile.Description), profile.Identifier));
			}
			string regions = string.Format("[{0}]", string.Join(", ", regionStrings));
			string privileged = IsPrivileged(request.ClientAddress) ? "true" : "false";
			string content = IndexContents;
			content = content.Replace("$REGIONS", regions);
			content = content.Replace("$PRIVILEGED", privileged);
			content = content.Replace("$REVISION", Assembly.GetEntryAssembly().GetName().Version.Revision.ToString());
			return new Reply(content);
		}
	}
}
