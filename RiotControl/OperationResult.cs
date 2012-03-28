using System.Collections.Generic;

namespace RiotControl
{
	public enum OperationResult
	{
		//The query succeeded
		Success,
		//The target of the operation was not found (invalid summoner name/account ID)
		NotFound,
		//The RPC operation timed out
		Timeout,
		//The worker wasn't connected to a server when the request was performed
		NotConnected,
	}

	static class WorkerResultExtension
	{
		static Dictionary<OperationResult, string> WorkerResultStrings = new Dictionary<OperationResult, string>()
		{
			{OperationResult.Success, "Success"},
			{OperationResult.NotFound, "NotFound"},
			{OperationResult.Timeout, "Timeout"},
			{OperationResult.NotConnected, "NotConnected"},
		};

		public static string GetString(this OperationResult result)
		{
			return WorkerResultStrings[result];
		}
	}
}
