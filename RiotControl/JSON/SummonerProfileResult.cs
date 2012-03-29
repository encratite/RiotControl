namespace RiotControl
{
	public class SummonerProfileResult
	{
		public string Result;
		public SummonerProfile Profile;

		public SummonerProfileResult(OperationResult result)
		{
			Result = result.GetString();
			Profile = null;
		}

		public SummonerProfileResult(SummonerProfile profile)
		{
			Result = OperationResult.Success.ToString();
			Profile = profile;
		}
	}
}
