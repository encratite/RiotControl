namespace RiotControl
{
	//This class is used exclusively for searches by summoner name submitted by users
	class LookupJob : Job
	{
		public string SummonerName;

		//If the lookup succeeds, the real summoner name, the account ID and the level are stored in this object
		public string RealSummonerName;
		public int AccountId;
		public int SummonerLevel;

		//Name of the summoner to be looked up
		public LookupJob(string summonerName)
			: base()
		{
			SummonerName = summonerName;
		}

		public void ProvideResult(string summonerName, int accountId, int summonerLevel)
		{
			RealSummonerName = summonerName;
			AccountId = accountId;
			SummonerLevel = summonerLevel;
			ProvideResult(JobQueryResult.Success);
		}
	}
}
