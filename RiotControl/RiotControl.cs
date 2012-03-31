using System.Linq;
using System.Windows.Forms;

namespace RiotControl
{
	class RiotControl
	{
		Configuration Configuration;
		StatisticsService StatisticsService;
		WebService WebService;

		MainForm MainForm;

		public RiotControl(Configuration configuration)
		{
			Configuration = configuration;
			Database databaseProvider = new Database(configuration.Database);
			StatisticsService = new StatisticsService(this, configuration, databaseProvider);
			WebService = new WebService(this, configuration, StatisticsService, databaseProvider);

			MainForm = new MainForm();
			MainForm.SetRegions((from x in configuration.RegionProfiles select x.Description).ToList());
		}

		public void Run()
		{
			StatisticsService.Run();
			WebService.Run();
			Application.Run(MainForm);
		}

		public void WriteLine(string line, params object[] arguments)
		{
			MainForm.WriteLine(string.Format(line, arguments));
		}
	}
}
