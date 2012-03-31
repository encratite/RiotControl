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
		bool IsFirstLine;

		public MainWindow(Configuration configuration)
		{
			InitializeComponent();

			IsFirstLine = true;

			DataContext = new MainWindowDataContext(configuration);
		}

		public void AppendText(string text)
		{
			if (IsFirstLine)
				IsFirstLine = false;
			else
				text = "\n" + text;
			OutputTextBox.Dispatcher.Invoke(
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
	}
}
