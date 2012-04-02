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
			PrivilegedAddresses = new List<string>();
		}
	}
}
