using System;
using System.Collections.Generic;
using System.Data.Common;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		Handler ApiSearchHandler;
		Handler ApiUpdateHandler;
		Handler ApiSummonerProfileHandler;
		Handler ApiSummonerGamesHandler;
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

			ApiSummonerGamesHandler = new Handler("Games", ApiSummonerGames, ArgumentType.String, ArgumentType.Integer);
			apiContainer.Add(ApiSummonerGamesHandler);

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
			Summoner summoner = Statistics.GetSummoner(worker.WorkerRegion, accountId);
			if (summoner != null)
			{
				using (var connection = GetConnection())
				{
					SummonerProfile profile = GetSummonerProfile(summoner, connection);
					output = new SummonerProfileResult(profile);
				}
			}
			else
				output = new SummonerProfileResult(OperationResult.NotFound);
			return GetJSONReply(output);
		}

		Reply ApiSummonerGames(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			SummonerGamesResult output;
			Summoner summoner = Statistics.GetSummoner(worker.WorkerRegion, accountId);
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

		Reply ApiSetAutomaticUpdates(Request request)
		{
			PrivilegeCheck(request);
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			bool updateAutomatically = (int)request.Arguments[2] != 0;
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			SummonerAutomaticUpdatesResult output;
			Summoner summoner = Statistics.GetSummoner(worker.WorkerRegion, accountId);
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
			foreach (var profile in ProgramConfiguration.RegionProfiles)
			{
				//Skip profiles for which no login has been specified
				if (profile.Login == null)
					continue;
				regionStrings.Add(string.Format("[{0}, {1}]", GetJavaScriptString(profile.Abbreviation), GetJavaScriptString(profile.Description)));
			}
			string regions = string.Format("[{0}]", string.Join(", ", regionStrings));
			string privileged = IsPrivileged(request.ClientAddress) ? "true" : "false";
			string content = IndexContents;
			content = content.Replace("$REGIONS", regions);
			content = content.Replace("$PRIVILEGED", privileged);
			return new Reply(content);
		}
	}
}
