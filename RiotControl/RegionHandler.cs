using System.Collections.Generic;

namespace RiotControl
{
	class RegionHandler
	{
		Configuration ServiceConfiguration;
		EngineRegionProfile Profile;
		DatabaseConnectionProvider DatabaseProvider;

		List<Worker> Workers;

		//Job queues
		Queue<LookupJob> LookupJobs;
		Queue<UpdateJob> ManualUpdateJobs;
		Queue<UpdateJob> AutomaticUpdateJobs;

		//This map holds locks for the account IDs that are currently being worked on
		//This way we can avoid updating an account from multiple workers simultaneously, causing concurrency issues with database updates
		public Dictionary<int, AccountLock> AccountLocks;

		public RegionHandler(Configuration configuration, EngineRegionProfile regionProfile, DatabaseConnectionProvider databaseProvider)
		{
			ServiceConfiguration = configuration;
			Profile = regionProfile;
			DatabaseProvider = databaseProvider;

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
				Worker newWorker = new Worker(Profile, login, ServiceConfiguration, this, DatabaseProvider);
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

		public AccountLock GetAccountLock(int accountId)
		{
			lock (AccountLocks)
			{
				AccountLock accountLock;
				if (AccountLocks.TryGetValue(accountId, out accountLock))
					accountLock.Counter += 1;
				else
				{
					accountLock = new AccountLock();
					AccountLocks[accountId] = accountLock;
				}
				return accountLock;
			}
		}

		public void ReleaseAccountLock(int accountId, AccountLock accountLock)
		{
			lock (AccountLocks)
			{
				accountLock.Counter -= 1;
				if (accountLock.Counter <= 0)
					AccountLocks.Remove(accountId);
			}
		}

		public bool MatchesAbbreviation(string abbreviation)
		{
			return Profile.Abbreviation == abbreviation;
		}
	}
}
