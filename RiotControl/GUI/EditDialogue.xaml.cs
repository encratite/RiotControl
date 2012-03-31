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
		bool UserProvidedNewLogin;

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

				UpdateOkButton();
			}
		}

		void UpdateOkButton()
		{
			OkButton.IsEnabled = UsernameTextBox.Text.Length > 0 && PasswordTextBox.Password.Length > 0;
		}

		public void OkButtonClick(object sender, EventArgs arguments)
		{
			UserProvidedNewLogin = true;
			Close();
		}

		public void CancelButtonClick(object sender, EventArgs arguments)
		{
			UserProvidedNewLogin = false;
			Close();
		}
	}
}
