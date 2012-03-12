namespace RiotControl
{
	class SummonerDescription
	{
		public string Name;
		public int Id;
		public int AccountId;

		public SummonerDescription(string name, int id, int accountId)
		{
			Name = name;
			Id = id;
			AccountId = accountId;
		}
	}
}
