using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiotControl
{
	public class RegionProfile
	{
		public string Abbreviation;
		public string RegionEnum;
		public string LoginQueueURL;
		public string RPCURL;

		public List<Login> Logins;
	}
}
