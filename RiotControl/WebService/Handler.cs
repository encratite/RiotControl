using System;
using System.Collections.Generic;
using System.Data.Common;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		Handler ApiSearchHandler;
		Handler ApiSummonerProfileHandler;
		Handler ApiSummonerGamesHandler;

		void InitialiseHandlers()
		{
			Handler apiContainer = new Handler("API");
			Server.Add(apiContainer);

			ApiSearchHandler = new Handler("Search", ApiSearch, ArgumentType.String, ArgumentType.String);
			apiContainer.Add(ApiSearchHandler);

			ApiSummonerProfileHandler = new Handler("Profile", ApiSummonerProfile, ArgumentType.String, ArgumentType.Integer);
			apiContainer.Add(ApiSummonerProfileHandler);

			ApiSummonerGamesHandler = new Handler("Games", ApiSummonerGames, ArgumentType.String, ArgumentType.Integer);
			apiContainer.Add(ApiSummonerGamesHandler);
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

		Reply ApiSummonerProfile(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			using (var connection = GetConnection())
			{
				SummonerDatabaseResult output;
				Summoner summoner = Statistics.GetSummoner(worker.WorkerRegion, accountId);
				if (summoner != null)
				{
					SummonerProfile profile = GetSummonerProfile(summoner, connection);
					output = new SummonerDatabaseResult(profile);
				}
				else
					output = new SummonerDatabaseResult(WorkerResult.NotFound);
				return GetJSONRepy(output);
			}
		}

		Reply ApiSummonerGames(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			using (var connection = GetConnection())
			{
				throw new Exception("Not implemented");
			}
		}
	}
}
