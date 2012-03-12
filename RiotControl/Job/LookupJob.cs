namespace RiotControl
{
	//This class is used exclusively for searches by summoner name submitted by users
	class LookupJob : Job
	{
		public string SummonerName;

		//If the lookup succeeds, the real summoner name and the account ID are stored in this object
		public string RealSummonerName;
		public int AccountID;

		//Name of the summoner to be looked up
		public LookupJob(string summonerName)
			: base()
		{
			SummonerName = summonerName;
		}

		public void ProvideResult(string summonerName, int accountId)
		{
			RealSummonerName = summonerName;
			AccountID = accountId;
			ProvideResult(JobQueryResult.Success);
		}
	}
}
