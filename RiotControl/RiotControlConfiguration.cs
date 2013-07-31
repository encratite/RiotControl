using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RiotControl
{
	[XmlRoot("Configuration")]
	public class RiotControlConfiguration : RiotGear.Configuration
    {

        // Controls whether the application should be minimised to the taskbar or the tray.
        public bool MinimiseToTray;

        // Controls wether the application should start minimised.
        public bool StartMinimised;

        public RiotControlConfiguration()
        {
            MinimiseToTray = true;
        }
    }
}
