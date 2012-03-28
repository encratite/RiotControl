using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiotControl
{
	public class SummonerGamesResult
	{
		public string Result;
		public List<ExtendedPlayer> Games;

		public SummonerGamesResult(OperationResult result)
		{
			Result = result.GetString();
			Games = null;
		}

		public SummonerGamesResult(List<ExtendedPlayer> games)
		{
			Result = OperationResult.Success.GetString();
			Games = games;
		}
	}
}
