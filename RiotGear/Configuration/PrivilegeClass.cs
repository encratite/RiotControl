using System.Collections.Generic;

namespace RiotGear
{
	public class PrivilegeClass
	{
		public List<string> EnabledAPIFunctions;
		public bool MatchAllAddresses;
		public List<string> Addresses;

		public PrivilegeClass()
		{
			EnabledAPIFunctions = new List<string>();
			MatchAllAddresses = false;
			Addresses = new List<string>();
		}
	}
}
