using System;
using System.Collections.Generic;

namespace RiotControl
{
	class RegionHandler
	{
		Configuration EngineConfiguration;
		EngineRegionProfile Profile;

		List<Worker> Workers;

		//This set holds the account IDs that are currently being worked on
		//This way we can avoid updating an account from multiple workers simultaneously, causing concurrency issues with database updates
		public HashSet<int> ActiveAccountIds;

		public object RegionLock;

		public RegionHandler(Configuration configuration, EngineRegionProfile regionProfile)
		{
			EngineConfiguration = configuration;
			Profile = regionProfile;

			ActiveAccountIds = new HashSet<int>();

			RegionLock = new object();

			Run();
		}

		public void Run()
		{
			Workers = new List<Worker>();
			foreach (var login in Profile.Logins)
			{
				Worker newWorker = new Worker(EngineConfiguration, Profile, login, this);
				Workers.Add(newWorker);
			}
		}
	}
}
