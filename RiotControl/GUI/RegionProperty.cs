using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace RiotControl
{
	class RegionProperty : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public EngineRegionProfile Profile
		{
			get;
			private set;
		}

		string description;
		bool hasLogin;

		public string Description
		{
			get
			{
				return description;
			}

			set
			{
				description = value;
				Notify("Description");
			}
		}

		public bool HasLogin
		{
			get
			{
				return hasLogin;
			}

			set
			{
				hasLogin = value;
				Notify("HasLogin");
			}
		}

		public RegionProperty(EngineRegionProfile profile)
		{
			Profile = profile;

			Description = profile.Description;
			SetHasLogin();
		}

		private void Notify(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public void SetHasLogin()
		{
			HasLogin = Profile.Login != null;
		}
	}
}