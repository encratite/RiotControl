using System;

namespace RiotGear
{
	//This interface is responsible for processing output and exceptions from the different services in a centralised way

	public interface IGlobalHandler
	{
		void WriteLine(string line, params object[] arguments);
		void HandleException(Exception exception);
	}
}
