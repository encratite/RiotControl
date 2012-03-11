namespace RiotControl
{
	class Summoner
	{
		public string Name;
		public int Id;
		public int AccountId;

		public Summoner(string name, int id, int accountId)
		{
			Name = name;
			Id = id;
			AccountId = accountId;
		}
	}
}
