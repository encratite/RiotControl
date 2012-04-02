namespace RiotGear
{
	class ProfileEntry
	{
		public string Activity;
		long TotalDuration;
		public int Count;

		public ProfileEntry(string activity)
		{
			Activity = activity;
			TotalDuration = 0;
			Count = 0;
		}

		public double AverageDuration()
		{
			return (double)TotalDuration / Count;
		}

		public void Add(long duration)
		{
			TotalDuration += duration;
			Count += 1;
		}
	}
}
