namespace RiotGear
{
	public class UpdateConfiguration
	{
		public string UpdateURL;
		public string ReleasePattern;

		public void Check()
		{
			Configuration.Check("Updates.UpdateURL", UpdateURL);
			Configuration.Check("Updates.ReleasePattern", ReleasePattern);
		}
	}
}
