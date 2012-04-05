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

			//Adding a default privileged address causes them to pile up as the XML serialiser just appends them instead of replacing the container
			PrivilegedAddresses = new List<string>();
		}
	}
}
