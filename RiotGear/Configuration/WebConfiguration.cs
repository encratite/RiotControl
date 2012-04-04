using System.Collections.Generic;

namespace RiotGear
{
	public class WebConfiguration
	{
		public string Host;
		public int Port;

		public bool EnableReverseProxyRealIPMode;

		public List<string> PrivilegedAddresses;

		public WebConfiguration()
		{
			Host = "127.0.0.1";
			Port = 80;

			EnableReverseProxyRealIPMode = false;

			PrivilegedAddresses = new List<string>()
			{
				Host,
			};
		}
	}
}
