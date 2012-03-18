using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Nil;

namespace RiotControl
{
	class Profiler
	{
		Dictionary<string, ProfileEntry> Profiles;

		long Timestamp;
		ProfileEntry CurrentProfile;
		int TotalExecutions;

		bool LiveOutput;
		string LiveOutputPrefix;

		public Profiler(bool liveOutput = false, string liveOutputPrefix = null)
		{
			LiveOutput = liveOutput;
			LiveOutputPrefix = liveOutputPrefix;

			Profiles = new Dictionary<string, ProfileEntry>();
			TotalExecutions = 0;
		}

		public void Start(string activity)
		{
			Timestamp = DateTime.Now.Ticks;
			if (!Profiles.TryGetValue(activity, out CurrentProfile))
			{
				CurrentProfile = new ProfileEntry(activity);
				Profiles[activity] = CurrentProfile;
			}	
		}

		public void Stop()
		{
			long duration = DateTime.Now.Ticks - Timestamp;
			CurrentProfile.Add(duration);
			TotalExecutions++;
			if (LiveOutput)
				Output.WriteLine("{0} [{1}] {2}: {3:F1} ms", Time.Timestamp(), LiveOutputPrefix, CurrentProfile.Activity, duration / 10000.0);
		}

		public void WriteLog(string path)
		{
			string output = "";
			List<ProfileEntry> profiles = Profiles.Values.ToList();
			profiles.Sort((ProfileEntry a, ProfileEntry b) => - a.AverageDuration().CompareTo(b.AverageDuration()));
			foreach (var profile in profiles)
				output += string.Format("{0}\n{1:F1} ms, {2} executions ({3:F1}%)\n", profile.Activity, profile.AverageDuration() / 10000, profile.Count, (double)profile.Count / TotalExecutions * 100.0);
			System.IO.File.WriteAllText(path, output);
		}
	}
}
