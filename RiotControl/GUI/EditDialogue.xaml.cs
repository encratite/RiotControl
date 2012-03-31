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

namespace RiotControl.GUI
{
	public partial class EditDialogue : Window
	{
		EngineRegionProfile Profile;

		public bool UserProvidedNewLogin
		{
			get;
			private set;
		}

		public EditDialogue(EngineRegionProfile profile)
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
			UserProvidedNewLogin = false;
			Close();
		}
	}
}
