using System.Collections.Generic;

namespace RiotControl
{
	class RegionHandler
	{
		Configuration EngineConfiguration;
		EngineRegionProfile Profile;

		List<Worker> Workers;

		//Job queues
		Queue<LookupJob> LookupJobs;
		Queue<UpdateJob> ManualUpdateJobs;
		Queue<UpdateJob> AutomaticUpdateJobs;

		//This map holds locks for the account IDs that are currently being worked on
		//This way we can avoid updating an account from multiple workers simultaneously, causing concurrency issues with database updates
		public Dictionary<int, AccountLock> AccountLocks;

		public RegionHandler(Configuration configuration, EngineRegionProfile regionProfile)
		{
			EngineConfiguration = configuration;
			Profile = regionProfile;

			LookupJobs = new Queue<LookupJob>();
			ManualUpdateJobs = new Queue<UpdateJob>();
			AutomaticUpdateJobs = new Queue<UpdateJob>();

			AccountLocks = new Dictionary<int, AccountLock>();

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

		//Workers sleep most of the time and need to be signaled using this method to notify them about the arrival of new jobs
		void ActivateWorkers()
		{
			foreach (var worker in Workers)
				worker.Notify();
		}

		public LookupJob PerformSummonerLookup(string summonerName)
		{
			LookupJob job = new LookupJob(summonerName);
			lock (LookupJobs)
				LookupJobs.Enqueue(job);
			ActivateWorkers();
			job.Execute();
			return job;
		}

		public UpdateJob PerformManualSummonerUpdate(int accountId)
		{
			UpdateJob job = new UpdateJob(accountId);
			lock (ManualUpdateJobs)
				ManualUpdateJobs.Enqueue(job);
			ActivateWorkers();
			job.Execute();
			return job;
		}

		public LookupJob GetLookupJob()
		{
			lock (LookupJobs)
			{
				if (LookupJobs.Count == 0)
					return null;
				return LookupJobs.Dequeue();
			}
		}

		UpdateJob GetUpdateJob(Queue<UpdateJob> queue)
		{
			lock (queue)
			{
				if (queue.Count == 0)
					return null;
				return queue.Dequeue();
			}
		}

		public UpdateJob GetManualUpdateJob()
		{
			return GetUpdateJob(ManualUpdateJobs);
		}

		public UpdateJob GetAutomaticUpdateJob()
		{
			return GetUpdateJob(AutomaticUpdateJobs);
		}

		public AccountLock GetAccountLock(int accountID)
		{
			lock (AccountLocks)
			{
				if (AccountLocks.ContainsKey(accountID))
				{
					AccountLock accountLock = AccountLocks[accountID];
					accountLock.Counter += 1;
					return accountLock;
				}
				else
				{
					AccountLock accountLock = new AccountLock();
					AccountLocks[accountID] = accountLock;
					return accountLock;
				}
			}
		}

		public void ReleaseAccountLock(int accountID, AccountLock accountLock)
		{
			lock (AccountLocks)
			{
				accountLock.Counter -= 1;
				if (accountLock.Counter <= 0)
					AccountLocks.Remove(accountID);
			}
		}
	}
}
