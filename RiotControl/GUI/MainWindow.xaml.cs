using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

using RiotGear;

namespace RiotControl
{
	public partial class MainWindow : Window
	{
		const string Website = "http://riot.cont.ro.lt/";

		Configuration Configuration;
		StatisticsService StatisticsService;
		Program Program;

		bool IsFirstLine;

		bool ShuttingDown;

		NotifyIcon TrayIcon;

		public MainWindow(Configuration configuration, Program program, StatisticsService statisticsService)
		{
			InitializeComponent();

			Configuration = configuration;
			Program = program;
			StatisticsService = statisticsService;

			IsFirstLine = true;

			ShuttingDown = false;

			DataContext = new MainWindowDataContext(configuration);

			Assembly entryAssembly = Assembly.GetEntryAssembly();
			Version version = entryAssembly.GetName().Version;

			Title = string.Format("Riot Control r{0}", version.Revision);
			RevisionLabel.Content = string.Format("r{0}", version.Revision);
			TimeLabel.Content = Nil.Assembly.GetAssemblyBuildTime(entryAssembly).ToString();
			WebsiteLabel.Content = Website;

			UpdateHelpLabel();

			var iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Icon.ico")).Stream;

			TrayIcon = new NotifyIcon();
			TrayIcon.Icon = new Icon(iconStream);
			TrayIcon.MouseDoubleClick += TrayIconDoubleClick;

			iconStream.Close();

			LoadOptions();
		}

		public void WriteLine(string line, params object[] arguments)
		{
			//Check if the application is shutting down to prevent timed invokes on the output text from piling up
			if (ShuttingDown)
				return;

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

		void OnClosing(object sender, EventArgs arguments)
		{
			Process.GetCurrentProcess().Kill();
		}

		void RegionGridOnSelectionChanged(object sender, EventArgs arguments)
		{
			EditButton.IsEnabled = true;
		}

		void EditButtonOnClick(object sender, EventArgs arguments)
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

		void BrowserButtonOnClick(object sender, EventArgs arguments)
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

		void WebsiteLabelClick(object sender, EventArgs arguments)
		{
			Process.Start(Website);
		}

		void UpdateHelpLabel()
		{
			if((from x in Configuration.RegionProfiles where x.Login != null select x).Count() > 0)
				HelpLabel.Content = "You need to access this service through your browser to look up summoners.";
			else
				HelpLabel.Content = "You need to set up at least one League of Legends account in the \"Logins\" tab.";
		}

		public void StartUpdate(ApplicationVersion newVersion)
		{
			var action = (Action)delegate
			{
				UpdateTabItem.IsEnabled = true;
				DownloadLabel.Content = string.Format("Downloading {0}...", newVersion.Filename);
				UpdateTabItem.Focus();
			};

			UpdateTabItem.Dispatcher.Invoke(action);
		}

		public void DownloadProgressUpdate(DownloadProgressChangedEventArgs arguments)
		{
			var action = (Action)delegate
			{
				DownloadProgressBar.Value = arguments.ProgressPercentage;
				ProgressLabel.Content = string.Format("Downloaded: {0}/{1}", Nil.String.GetFileSizeString(arguments.BytesReceived), Nil.String.GetFileSizeString(arguments.TotalBytesToReceive));
			};

			UpdateTabItem.Dispatcher.Invoke(action);
		}

		public void DownloadError(Exception exception)
		{
			var action = (Action)delegate
			{
				string message;
				if (exception.InnerException == null)
					message = exception.Message;
				else
					message = exception.InnerException.Message;
				DownloadLabel.Content = string.Format("An error occurred: {0}", message);
			};

			UpdateTabItem.Dispatcher.Invoke(action);
		}

		void TrayIconDoubleClick(object sender, MouseEventArgs arguments)
		{
			WindowState = WindowState.Normal;
		}

		void OnStateChanged(object sender, EventArgs arguments)
		{
			if (Configuration.MinimiseToTray)
			{
				if (WindowState == WindowState.Minimized)
				{
					ShowInTaskbar = false;
					TrayIcon.Visible = true;
				}
				else if (WindowState == WindowState.Normal)
				{
					ShowInTaskbar = true;
					TrayIcon.Visible = false;
				}
			}
		}

		void LoadOptions()
		{
			WebServerAddressBox.Text = Configuration.Web.Host;
			WebServerPortBox.Text = Configuration.Web.Port.ToString();

			UpdateIntervalBox.Text = Configuration.AutomaticUpdateInterval.ToString();

			MinimiseToTrayCheckbox.IsChecked = Configuration.MinimiseToTray;

			StartWithWindowsCheckbox.IsChecked = RegistryHandler.IsAutorun();
		}

		int GetInteger(string description, System.Windows.Controls.TextBox box)
		{
			try
			{
				return Convert.ToInt32(box.Text);
			}
			catch (FormatException)
			{
				throw new Exception(string.Format("Invalid value in field \"{0}\".\nThe configuration has not been saved.", description));
			}
		}

		void SaveOptions()
		{
			try
			{
				if (StartWithWindowsCheckbox.IsChecked.Value)
					RegistryHandler.EnableAutorun();
				else
					RegistryHandler.DisableAutorun();

				int port = GetInteger("Web server port", WebServerPortBox);
				int updateInterval = GetInteger("Update interval", UpdateIntervalBox);

				Configuration.Web.Host = WebServerAddressBox.Text;
				Configuration.Web.Port = port;

				Configuration.AutomaticUpdateInterval = updateInterval;

				Configuration.MinimiseToTray = MinimiseToTrayCheckbox.IsChecked.Value;

				Program.SaveConfiguration();

				System.Windows.MessageBox.Show("Your configuration has been saved.", "Configuration saved");

			}
			catch (Exception exception)
			{
				System.Windows.MessageBox.Show(exception.Message, "Configuration error");
			}
		}

		void SaveButtonClick(object sender, EventArgs arguments)
		{
			SaveOptions();
		}

		void ResetButtonClick(object sender, EventArgs arguments)
		{
			LoadOptions();
		}
	}
}
