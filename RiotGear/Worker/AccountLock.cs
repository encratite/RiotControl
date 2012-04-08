namespace RiotGear
{
	class AccountLock
	{
		//This property holds the number of workers operating 
		public int Counter;

		public AccountLock()
		{
			Counter = 1;
		}
	}
}