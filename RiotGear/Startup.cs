using System.IO;
using System.Reflection;

namespace RiotGear
{
	public static class Startup
	{
		//Set the current directory to the directory of the application
		//This is necessary for cases where the application is launched from the wrong directory
		public static void SetCurrentDirectory()
		{
			string directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); ;
			Directory.SetCurrentDirectory(directory);
		}
	}
}
