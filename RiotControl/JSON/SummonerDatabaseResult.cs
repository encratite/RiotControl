namespace RiotControl
{
	public class SummonerDatabaseResult
	{
		public string Result;
		public Summoner SummonerData;

		public SummonerDatabaseResult(WorkerResult result)
		{
			Result = result.GetString();
			SummonerData = null;
		}

		public SummonerDatabaseResult(Summoner summonerData)
		{
			Result = WorkerResult.Success.ToString();
			SummonerData = summonerData;
		}
	}
}
