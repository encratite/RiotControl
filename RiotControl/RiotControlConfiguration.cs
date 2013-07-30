using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RiotControl
{
	//[XmlType(TypeName = "Configuration")]
	[XmlRoot("Configuration")]
	public class RiotControlConfiguration : RiotGear.Configuration
    {

        // Controls whether the application should be minimised to the taskbar or the tray
        public bool MinimiseToTray;

        // Controls wether the application should start minimized.
        public bool StartMinimized;

        public RiotControlConfiguration()
        {
            MinimiseToTray = true;
        }


    }
}
