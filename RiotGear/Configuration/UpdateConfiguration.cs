using System.Collections.Generic;

namespace RiotGear
{
	public class UpdateConfiguration
	{
		public bool EnableAutomaticUpdates;
		public string UpdateURL;
		public string ReleasePattern;
		public List<string> UpdateTargets;

		public UpdateConfiguration()
		{
			EnableAutomaticUpdates = true;
			UpdateTargets = new List<string>();
		}

		public void Check()
		{
			Configuration.Check("Updates.UpdateURL", UpdateURL);
			Configuration.Check("Updates.ReleasePattern", ReleasePattern);
		}
	}
}
