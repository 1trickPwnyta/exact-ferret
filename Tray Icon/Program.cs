using Exact_Ferret;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tray_Icon
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Also start the Exact Ferret background process, if not started already
            if (!Communication.ping(0))
            {
                Communication.startExactFerret(0);
            }
            if (!Communication.ping(1))
            {
                Communication.startExactFerret(1); // it will kill itself if not enabled
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TrayContext context = new TrayContext();
            Application.Run(context);
        }
    }
}
