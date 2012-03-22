using System;
using System.Collections.Generic;
using System.Data.Common;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		Handler APISearchHandler;
		Handler APIDatabaseHandler;

		void InitialiseHandlers()
		{
			Handler apiContainer = new Handler("API");
			Server.Add(apiContainer);

			APISearchHandler = new Handler("Search", APISearch, ArgumentType.String, ArgumentType.String);
			apiContainer.Add(APISearchHandler);

			APIDatabaseHandler = new Handler("Database", APIDatabase, ArgumentType.String, ArgumentType.Integer);
			apiContainer.Add(APIDatabaseHandler);
		}

		Reply APISearch(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			string summonerName = (string)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			int accountId = 0;
			WorkerResult result = worker.UpdateSummonerByName(summonerName, ref accountId);
			SummonerSearchResult output;
			if (result == WorkerResult.Success)
				output = new SummonerSearchResult(accountId);
			else
				output = new SummonerSearchResult(result);
			return GetJSONRepy(output);
		}

		Reply APIDatabase(Request request)
		{
			var arguments = request.Arguments;
			string regionAbbreviation = (string)request.Arguments[0];
			int accountId = (int)request.Arguments[1];
			Worker worker = GetWorkerByAbbreviation(regionAbbreviation);
			using (var connection = GetConnection())
			{
				SummonerDatabaseResult output;
				Summoner summoner = LoadSummoner(regionAbbreviation, accountId, connection);
				if (summoner == null)
					output = new SummonerDatabaseResult(WorkerResult.NotFound);
				else
					output = new SummonerDatabaseResult(summoner);
				return GetJSONRepy(output);
			}
		}
	}
}
