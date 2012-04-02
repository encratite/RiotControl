namespace RiotGear
{
	public class SummonerProfileResult
	{
		public string Result;
		public Summoner Summoner;

		public SummonerProfileResult(OperationResult result)
		{
			Result = result.GetString();
			Summoner = null;
		}

		public SummonerProfileResult(Summoner summoner)
		{
			Result = OperationResult.Success.ToString();
			Summoner = summoner;
		}
	}
}
