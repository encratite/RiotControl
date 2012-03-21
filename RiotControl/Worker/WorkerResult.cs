using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiotControl
{
	public enum WorkerResult
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
		static Dictionary<WorkerResult, string> WorkerResultStrings = new Dictionary<WorkerResult, string>()
		{
			{WorkerResult.Success, "Success"},
			{WorkerResult.NotFound, "NotFound"},
			{WorkerResult.Timeout, "Timeout"},
			{WorkerResult.NotConnected, "NotConnected"},
		};

		public static string GetString(this WorkerResult result)
		{
			return WorkerResultStrings[result];
		}
	}
}
