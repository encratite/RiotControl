using System;
using System.Collections.Generic;
using System.Data.Common;

using Blighttp;

namespace RiotControl
{
	partial class WebService
	{
		Handler APISearchHandler;

		void InitialiseHandlers()
		{
			APISearchHandler = new Handler("Search", APISearch, ArgumentType.String, ArgumentType.String);
			Server.Add(APISearchHandler);
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
	}
}
