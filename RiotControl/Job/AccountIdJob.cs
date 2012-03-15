namespace RiotControl
{
	//This class is used for both update requests submitted by users and automatic updates
	class AccountIdJob : Job
	{
		public int AccountId;

		//Account ID of the summoner to be updated
		public AccountIdJob(int accountId)
			: base()
		{
			AccountId = accountId;
		}
	}
}
