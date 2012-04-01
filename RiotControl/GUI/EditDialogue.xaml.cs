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
using System.Windows.Shapes;

namespace RiotControl
{
	public partial class EditDialogue : Window
	{
		EngineRegionProfile Profile;

		public bool UserProvidedNewLogin
		{
			get;
			private set;
		}

		public EditDialogue(EngineRegionProfile profile, MainWindow parent)
		{
			Profile = profile;

			InitializeComponent();

			Title = "Edit login for " + profile.Description;
			Login login = profile.Login;
			if (login != null)
			{
				UsernameTextBox.Text = login.Username;
				PasswordTextBox.Password = login.Password;
			}

			//Center it
			Left = parent.Left + (parent.Width  - Width) / 2;
			Top = parent.Top + (parent.Height - Height) / 2;

			//Default to false, this way nothing special needs to be done when the user closes the window without using the Ok/Cancel button
			UserProvidedNewLogin = false;
		}

		public void OkButtonClick(object sender, EventArgs arguments)
		{
			//Obtain a lock on the profile to avoid race conditions with the worker that might be using this data
			lock (Profile)
			{
				UserProvidedNewLogin = true;
				string username = UsernameTextBox.Text;
				string password = PasswordTextBox.Password;
				if (username.Length > 0)
					Profile.Login = new Login(username, password);
				else
					Profile.Login = null;
			}
			Close();
		}

		public void CancelButtonClick(object sender, EventArgs arguments)
		{
			Close();
		}
	}
}
