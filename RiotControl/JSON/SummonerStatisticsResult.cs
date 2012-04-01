namespace RiotControl
{
	public class SummonerStatisticsResult
	{
		public string Result;
		public SummonerStatistics Statistics;

		public SummonerStatisticsResult(OperationResult result)
		{
			Result = result.GetString();
			Statistics = null;
		}

		public SummonerStatisticsResult(SummonerStatistics statistics)
		{
			Result = OperationResult.Success.ToString();
			Statistics = statistics;
		}
	}
}
