namespace RiotGear
{
	public class SummonerAutomaticUpdatesResult
	{
		public string Result;

		public SummonerAutomaticUpdatesResult(OperationResult result)
		{
			Result = result.GetString();
		}
	}
}
