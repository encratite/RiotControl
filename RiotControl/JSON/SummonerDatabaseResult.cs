namespace RiotControl
{
	public class SummonerDatabaseResult
	{
		public string Result;
		public SummonerProfile Profile;

		public SummonerDatabaseResult(WorkerResult result)
		{
			Result = result.GetString();
			Profile = null;
		}

		public SummonerDatabaseResult(SummonerProfile profile)
		{
			Result = WorkerResult.Success.ToString();
			Profile = profile;
		}
	}
}
