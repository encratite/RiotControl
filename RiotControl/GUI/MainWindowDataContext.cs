using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace RiotControl
{
	class MainWindowDataContext
	{
		public ICollectionView Regions { get; private set; }

		public MainWindowDataContext(Configuration configuration)
		{
			List<RegionProperty> regions = (from x in configuration.RegionProfiles select new RegionProperty(x)).ToList();
			Regions = CollectionViewSource.GetDefaultView(regions);
		}
	}
}
