using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiotControl
{
	class SummonerUpdateResult
	{
		public string Result;

		public SummonerUpdateResult(AccountIdJob job)
		{
			switch (job.Result)
			{
				case JobQueryResult.Success:
					Result = "Success";
					break;

				case JobQueryResult.NotFound:
					Result = "NotFound";
					break;

				case JobQueryResult.Timeout:
					Result = "Timeout";
					break;
			}
		}
	}
}
