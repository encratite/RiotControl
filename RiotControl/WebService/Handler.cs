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

			IndexHandler = new Handler(Index);
			Server.Add(IndexHandler);
		}

		Reply ApiSearch(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			string summonerName = (string)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			Summoner summoner = null;
			WorkerResult result = worker.FindSummoner(summonerName, ref summoner);
			SummonerSearchResult output;
			if (result == WorkerResult.Success)
				output = new SummonerSearchResult(summoner.AccountId);
			else
				output = new SummonerSearchResult(result);
			return GetJSONRepy(output);
		}

		Reply ApiUpdateSummoner(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			WorkerResult result = worker.UpdateSummonerByAccountId(accountId);
			SummonerUpdateResult output = new SummonerUpdateResult(result);
			return GetJSONRepy(output);
		}

		Reply ApiSummonerProfile(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			SummonerDatabaseResult output;
			Summoner summoner = Statistics.GetSummoner(worker.WorkerRegion, accountId);
			if (summoner != null)
			{
				using (var connection = GetConnection())
				{
					SummonerProfile profile = GetSummonerProfile(summoner, connection);
					output = new SummonerDatabaseResult(profile);
				}
			}
			else
				output = new SummonerDatabaseResult(WorkerResult.NotFound);
			return GetJSONRepy(output);
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
				output = new SummonerGamesResult(WorkerResult.NotFound);
			return GetJSONRepy(output);
		}

		Reply Index(Request request)
		{
			return new Reply(IndexContents);
		}
	}
}
