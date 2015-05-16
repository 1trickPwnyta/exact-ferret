using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exact_Ferret.Utility_Classes
{
    enum WindowsVersion : int
    {
        Undefined, Windows2000, WindowsXP, WindowsXP64_Or_Server2003, WindowsVista_Or_Server2008, Windows7_Or_Server2008R2,
        Windows8_Or_Server2012, Windows81_Or_Server2012R2
    }

    class OSUtil
    {
        public static WindowsVersion getWindowsVersion()
        {
            double windowsVersion = double.Parse(Environment.OSVersion.Version.Major + "." + 
                    Environment.OSVersion.Version.Minor);

            if (windowsVersion < 5.09)
                return WindowsVersion.Windows2000;
            else if (windowsVersion < 5.19)
                return WindowsVersion.WindowsXP;
            else if (windowsVersion < 5.29)
                return WindowsVersion.WindowsXP64_Or_Server2003;
            else if (windowsVersion < 6.09)
                return WindowsVersion.WindowsVista_Or_Server2008;
            else if (windowsVersion < 6.19)
                return WindowsVersion.Windows7_Or_Server2008R2;
            else if (windowsVersion < 6.29)
                return WindowsVersion.Windows8_Or_Server2012;
            else if (windowsVersion < 6.39)
                return WindowsVersion.Windows81_Or_Server2012R2;
            else
                return WindowsVersion.Undefined;
        }
    }
}
