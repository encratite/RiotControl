namespace RiotControl
{
	public class SummonerDatabaseResult
	{
		public string Result;
		public SummonerProfile Profile;

		public SummonerDatabaseResult(OperationResult result)
		{
			Result = result.GetString();
			Profile = null;
		}

		public SummonerDatabaseResult(SummonerProfile profile)
		{
			Result = OperationResult.Success.ToString();
			Profile = profile;
		}
	}
}
