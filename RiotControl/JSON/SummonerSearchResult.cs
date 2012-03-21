namespace RiotControl
{
	public class SummonerSearchResult
	{
		public string Result;
		public int? AccountId;

		public SummonerSearchResult(WorkerResult result)
		{
			Result = result.GetString();
			AccountId = null;
		}

		public SummonerSearchResult(int accountId)
		{
			Result = WorkerResult.Success.GetString();
			AccountId = accountId;
		}
	}
}
