using System;
using System.Collections.Generic;

namespace RiotControl
{
	class RegionHandler
	{
		Configuration EngineConfiguration;
		EngineRegionProfile Profile;

		//This set holds the account IDs that are currently being worked on
		//This way we can avoid updating an account from multiple workers simultaneously, causing concurrency issues with database updates
		HashSet<int> ActiveAccountIds;

		List<Worker> Workers;

		public RegionHandler(Configuration configuration, EngineRegionProfile regionProfile)
		{
			EngineConfiguration = configuration;
			Profile = regionProfile;

			ActiveAccountIds = new HashSet<int>();

			Run();
		}

		public void Run()
		{
			Workers = new List<Worker>();
			foreach (var login in Profile.Logins)
			{
				Worker newWorker = new Worker(EngineConfiguration, Profile, login, ActiveAccountIds);
				Workers.Add(newWorker);
			}
		}
	}
}
