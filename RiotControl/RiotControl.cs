using System.Windows.Forms;

namespace RiotControl
{
	class RiotControl
	{
		StatisticsService StatisticsService;
		WebService WebService;

		MainForm MainForm;

		public RiotControl(Configuration configuration)
		{
			Database databaseProvider = new Database(configuration.Database);
			StatisticsService = new StatisticsService(this, configuration, databaseProvider);
			WebService = new WebService(this, configuration, StatisticsService, databaseProvider);

			MainForm = new MainForm();
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
