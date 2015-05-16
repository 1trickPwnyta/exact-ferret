using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret.Utility_Classes
{
    class ShortcutUtil
    {
        public static void createShortcut(string location, string target)
        {
            createShortcut(location, target, Path.GetDirectoryName(target), "", "", null);
        }

        public static void createShortcut(string location, string target, string arguments)
        {
            createShortcut(location, target, Path.GetDirectoryName(target), arguments, "", null);
        }

        public static void createShortcut(string location, string target, string workingDirectory, string arguments, string description, string hotKey)
        {
            Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); //Windows Script Host Shell Object
            dynamic shell = Activator.CreateInstance(t);
            try
            {
                var lnk = shell.CreateShortcut(location);
                try
                {
                    lnk.TargetPath = target;
                    lnk.WorkingDirectory = workingDirectory;
                    lnk.Arguments = arguments;
                    lnk.Description = description;
                    if (hotKey != null)
                        lnk.HotKey = hotKey;
                    lnk.Save();
                }
                finally
                {
                    Marshal.FinalReleaseComObject(lnk);
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(shell);
            }
        }

        public static int getShortcutHoyKey(string location)
        {
            try
            {
                string pathOnly = Path.GetDirectoryName(location);
                string filenameOnly = Path.GetFileName(location);

                Shell32.Shell shell = new Shell32.Shell();
                Shell32.Folder folder = shell.NameSpace(pathOnly);
                Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);

                if (folderItem != null)
                {
                    Shell32.ShellLinkObject link =
                    (Shell32.ShellLinkObject)folderItem.GetLink;
                    return link.Hotkey;
                }
            }
            catch { }

            return -1; // not found
        }
    }
}
