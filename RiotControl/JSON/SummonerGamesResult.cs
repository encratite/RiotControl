using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiotControl
{
	public class SummonerGamesResult
	{
		public string Result;
		public List<GameTeamPlayer> Games;

		public SummonerGamesResult(WorkerResult result)
		{
			Result = result.GetString();
			Games = null;
		}

		public SummonerGamesResult(List<GameTeamPlayer> games)
		{
			Result = WorkerResult.Success.GetString();
			Games = games;
		}
	}
}
