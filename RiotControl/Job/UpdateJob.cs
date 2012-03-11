namespace RiotControl
{
	//This class is used for both update requests submitted by users and automatic updates
	class UpdateJob : Job
	{
		public int AccountId;

		//Account ID of the summoner to be updated
		public UpdateJob(int accountId)
			: base()
		{
			AccountId = accountId;
		}
	}
}
