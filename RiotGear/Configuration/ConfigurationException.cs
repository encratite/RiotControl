using System;

namespace RiotGear
{
	public class ConfigurationException : Exception
	{
		public ConfigurationException(string message)
			: base(message)
		{
		}
	}
}
