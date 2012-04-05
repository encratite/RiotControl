using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using RiotGear;

namespace RiotControl
{
	public partial class MainWindow : Window
	{
		Configuration Configuration;
		StatisticsService StatisticsService;
		Program Program;

		bool IsFirstLine;

		public MainWindow(Configuration configuration, Program program, StatisticsService statisticsService)
		{
			InitializeComponent();

			Configuration = configuration;
			Program = program;
			StatisticsService = statisticsService;

			IsFirstLine = true;

			DataContext = new MainWindowDataContext(configuration);

			Assembly entryAssembly = Assembly.GetEntryAssembly();

			Version version = entryAssembly.GetName().Version;

			Title = string.Format("Riot Control r{0} ({1})", version.Revision, Nil.Assembly.GetAssemblyBuildTime(entryAssembly));

			UpdateHelpLabel();
		}

		public void WriteLine(string line, params object[] arguments)
		{
			line = string.Format(line, arguments);
			line = string.Format("{0} {1}", Nil.Time.Timestamp(), line);
			if (IsFirstLine)
				IsFirstLine = false;
			else
				line = "\n" + line;

			var action = (Action)delegate
			{
				lock (OutputTextBox)
				{
					OutputTextBox.AppendText(line);
					OutputTextBox.ScrollToEnd();
				}
			};

			OutputTextBox.Dispatcher.Invoke(action);
		}

		public void OnClosing(object sender, EventArgs arguments)
		{
			Program.Terminate();
			Environment.Exit(0);
		}

		public void RegionGridOnSelectionChanged(object sender, EventArgs arguments)
		{
			EditButton.IsEnabled = true;
		}

		public void EditButtonOnClick(object sender, EventArgs arguments)
		{
			RegionProperty region = (RegionProperty)RegionGrid.SelectedItem;
			EditDialogue editDialogue = new EditDialogue(region.Profile, this);
			editDialogue.ShowDialog();
			//The login might have been modified by the user so the grid needs to be updated
			region.SetHasLogin();
			//Have the statistics service check if new workers need to be added because of this update
			StatisticsService.AddMissingWorkers();
			//Save the new configuration
			Program.SaveConfiguration();
			//Update the help text
			UpdateHelpLabel();
		}

		public void BrowserButtonOnClick(object sender, EventArgs arguments)
		{
			string host = Configuration.Web.Host;
			if (host == null || host.Length == 0)
				host = "127.0.0.1";
			string url = "http://" + host;
			if (Configuration.Web.Port != 80)
				url += string.Format(":{0}", Configuration.Web.Port);
			url += "/";

			Process.Start(url);
		}

		public void UpdateHelpLabel()
		{
			if((from x in Configuration.RegionProfiles where x.Login != null select x).Count() > 0)
				HelpLabel.Content = "You need to access this service through your browser to look up summoners.";
			else
				HelpLabel.Content = "You need to set up at least one League of Legends account in the \"Logins\" tab.";
		}
	}
}
