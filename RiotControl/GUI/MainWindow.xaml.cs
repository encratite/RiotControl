using System;
using System.Collections.Generic;
using System.Linq;
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

namespace RiotControl
{
	public partial class MainWindow : Window
	{
		bool IsFirstLine;

		public MainWindow()
		{
			IsFirstLine = true;

			InitializeComponent();
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
	}
}
