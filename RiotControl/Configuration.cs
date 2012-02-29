using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiotControl
{
	public class Configuration
	{
		public DatabaseConfiguration Database;
		public List<RegionProfile> RegionProfiles;

		public Configuration()
		{
			Database = new DatabaseConfiguration();
			RegionProfiles = new List<RegionProfile>();
		}
	}
}
