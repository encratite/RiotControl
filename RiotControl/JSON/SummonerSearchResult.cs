namespace RiotControl
{
	public class SummonerSearchResult
	{
		public string Result;
		public int? AccountId;

		public SummonerSearchResult(OperationResult result)
		{
			Result = result.GetString();
			AccountId = null;
		}

		public SummonerSearchResult(int accountId)
		{
			Result = OperationResult.Success.GetString();
			AccountId = accountId;
		}
	}
}
