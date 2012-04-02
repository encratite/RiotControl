namespace RiotGear
{
	public class SummonerUpdateResult
	{
		public string Result;

		public SummonerUpdateResult(OperationResult result)
		{
			Result = result.GetString();
		}
	}
}
