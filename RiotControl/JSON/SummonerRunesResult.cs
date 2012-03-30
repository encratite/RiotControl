using System.Collections.Generic;

namespace RiotControl
{
	public class SummonerRunesResult
	{
		public string Result;
		public List<RunePage> RunePages;

		public SummonerRunesResult(OperationResult result)
		{
			Result = result.GetString();
			RunePages = null;
		}

		public SummonerRunesResult(List<RunePage> runePages)
		{
			Result = OperationResult.Success.ToString();
			RunePages = runePages;
		}
	}
}
