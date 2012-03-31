using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RiotControl
{
	public partial class MainForm : Form
	{
		const int WM_VSCROLL = 0x115;
		const int SB_BOTTOM = 7;

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr window, int message, int wparam, int lparam);

		bool IsFirstLine;

		public MainForm()
		{
			IsFirstLine = true;

			InitializeComponent();
		}

		void HandleCheck()
		{
			if (!IsHandleCreated)
				CreateHandle();
		}

		public void WriteLine(string line)
		{
			lock (outputTextBox)
			{
				HandleCheck();

				if (IsFirstLine)
					IsFirstLine = false;
				else
					line = "\n" + line;
				outputTextBox.Invoke
				(
					(MethodInvoker)delegate
					{
						outputTextBox.AppendText(line);
						SendMessage(outputTextBox.Handle, WM_VSCROLL, SB_BOTTOM, 0);
					}
				);
			}
		}

		public void SetRegions(List<string> regions)
		{
			HandleCheck();

			regionListBox.Invoke
				(
					(MethodInvoker)delegate
					{
						foreach (var region in regions)
						{
							regionListBox.Items.Add(region);
						}
					}
				);
		}

		private void MainFormFormClosed(object sender, FormClosedEventArgs eventArguments)
		{
			//Forcefully terminate the application
			Environment.Exit(0);
		}

		private void regionListBoxSelectedValueChanged(object sender, EventArgs e)
		{
			editRegionButton.Enabled = true;
		}
	}
}
