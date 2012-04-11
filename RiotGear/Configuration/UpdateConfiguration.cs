using System.Collections.Generic;

namespace RiotGear
{
	public class UpdateConfiguration
	{
		public bool EnableAutomaticUpdates;
		//This is just a debugging feature
		public bool ForceUpdate;
		public string UpdateURL;
		public string ReleasePattern;
		public List<string> UpdateTargets;

		public UpdateConfiguration()
		{
			EnableAutomaticUpdates = true;
			ForceUpdate = false;
			UpdateTargets = new List<string>();
		}

		public void Check()
		{
			Configuration.Check("Updates.UpdateURL", UpdateURL);
			Configuration.Check("Updates.ReleasePattern", ReleasePattern);
		}
	}
}
