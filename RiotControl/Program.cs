using System;
using System.Windows.Forms;

namespace RiotControl
{
	class Program
	{
		static string ConfigurationPath = "Configuration.xml";

		static void Main(string[] arguments)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Configuration configuration;
			try
			{
				Nil.Serialiser<Configuration> serialiser = new Nil.Serialiser<Configuration>(ConfigurationPath);
				configuration = serialiser.Load();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
				return;
			}

			RiotControl control = new RiotControl(configuration);
			control.Run();
		}
	}
}
