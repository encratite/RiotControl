using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace RiotControl
{
	class RegionProperty : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		string region;
		bool hasLogin;

		public string Region
		{
			get
			{
				return region;
			}

			set
			{
				region = value;
				Notify("Region");
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

		private void Notify(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	class MainWindowDataContext
	{
		public ICollectionView Regions { get; private set; }

		public MainWindowDataContext(Configuration configuration)
		{
			List<RegionProperty> regions = (from x in configuration.RegionProfiles select new RegionProperty { Region = x.Description, HasLogin = x.Login != null }).ToList();
			Regions = CollectionViewSource.GetDefaultView(regions);
		}
	}
}
