using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RiotControl
{
	public partial class MainWindow : Window
	{
		StatisticsService StatisticsService;
		Program Program;

		bool IsFirstLine;

		public MainWindow(Configuration configuration, Program program, StatisticsService statisticsService)
		{
			InitializeComponent();

			Program = program;
			StatisticsService = statisticsService;

			IsFirstLine = true;

			DataContext = new MainWindowDataContext(configuration);
		}

		public void AppendText(string text)
		{
			if (IsFirstLine)
				IsFirstLine = false;
			else
				text = "\n" + text;
			OutputTextBox.Dispatcher.Invoke
			(
				(Action)delegate
				{
					lock (OutputTextBox)
					{
						OutputTextBox.AppendText(text);
						OutputTextBox.ScrollToEnd();
					}
				}
			);
		}

		public void OnClosed(object sender, EventArgs arguments)
		{
			//Kill the application, SQLite has to suck it up
			Environment.Exit(0);
		}

		public void RegionGridOnSelectionChanged(object sender, EventArgs arguments)
		{
			EditButton.IsEnabled = true;
		}

		public void EditButtonOnClick(object sender, EventArgs arguments)
		{
			RegionProperty region = (RegionProperty)RegionGrid.SelectedItem;
			EditDialogue editDialogue = new EditDialogue(region.Profile);
			editDialogue.ShowDialog();
			//The login might have been modified by the user so the grid needs to be updated
			region.SetHasLogin();
			//Have the statistics service check if new workers need to be added because of this update
			StatisticsService.AddMissingWorkers();
			//Save the new configuration
			Program.SaveConfiguration();
		}
	}
}
