namespace RiotControl
{
	public class SummonerUpdateResult
	{
		public string Result;

		public SummonerUpdateResult(WorkerResult result)
		{
			Result = result.GetString();
		}
	}
}
