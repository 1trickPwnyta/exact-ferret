using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Logging;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using Exact_Ferret.Settings_Classes;
using Exact_Ferret.Utility_Classes;
using Search;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;

namespace Exact_Ferret
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        [STAThread]
        static void Main(string[] args)
        {
            // Initialize the Properties Manager
            PropertiesManager.initialize();

            // Initialize logging
            Log.setLogLevel(PropertiesManager.getLogLevel());
            Log.setLogFileLocation(PropertiesManager.getLogFilePath());
            Log.setLogRolloverKBytes(PropertiesManager.getLogRolloverKilobytes());

            if (args.Length > 0)
            {
                // Handle the command line argument
                string arg0 = args[0].ToLower();
                bool validArgs = true;
                switch (arg0)
                {
                    case "-settings":
                    case "-s":
                        // Open the settings window
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new SettingsForm());
                        break;
                    case "-quick":
                    case "-q":
                        // Perform a quick run
                        string term = null;
                        SearchOptions searchOptions = new SearchOptions();

                        // Get additional search options
                        for (int i = 1; i < args.Length; i++)
                        {
                            string arg = args[i].ToLower();
                            string argParam = null;
                            if (arg.StartsWith("-"))
                            {
                                if (args.Length > i + 1)
                                    argParam = args[++i];
                                else
                                {
                                    validArgs = false;
                                    break;
                                }
                            }

                            // For tryParse
                            int intSponge;
                            double doubleSponge;
                            bool boolSponge;

                            switch (arg)
                            {
                                case "-searchengine":
                                case "-se":
                                    if (argParam.ToLower().Equals(SearchOptions.GOOGLE.ToLower()))
                                        searchOptions.setSearchEngine(SearchOptions.GOOGLE);
                                    else if (argParam.ToLower().Equals(SearchOptions.BING.ToLower()))
                                        searchOptions.setSearchEngine(SearchOptions.BING);
                                    else
                                        validArgs = false;
                                    break;
                                case "-dictionary":
                                case "-d":
                                    searchOptions.setDictionaryFile(argParam);
                                    break;
                                case "-minwidth":
                                case "-nw":
                                    if (int.TryParse(argParam, out intSponge))
                                        searchOptions.addOption(new SearchOption(SearchOption.MINIMUM_WIDTH, argParam));
                                    else
                                        validArgs = false;
                                    break;
                                case "-minheight":
                                case "-nh":
                                    if (int.TryParse(argParam, out intSponge))
                                        searchOptions.addOption(new SearchOption(SearchOption.MINIMUM_HEIGHT, argParam));
                                    else
                                        validArgs = false;
                                    break;
                                case "-maxwidth":
                                case "-xw":
                                    if (int.TryParse(argParam, out intSponge))
                                        searchOptions.addOption(new SearchOption(SearchOption.MAXIMUM_WIDTH, argParam));
                                    else
                                        validArgs = false;
                                    break;
                                case "-maxheight":
                                case "-xh":
                                    if (int.TryParse(argParam, out intSponge))
                                        searchOptions.addOption(new SearchOption(SearchOption.MAXIMUM_HEIGHT, argParam));
                                    else
                                        validArgs = false;
                                    break;
                                case "-minratio":
                                case "-nr":
                                    if (double.TryParse(argParam, out doubleSponge))
                                        searchOptions.addOption(new SearchOption(SearchOption.MINIMUM_RATIO, argParam));
                                    else
                                        validArgs = false;
                                    break;
                                case "-maxratio":
                                case "-xr":
                                    if (double.TryParse(argParam, out doubleSponge))
                                        searchOptions.addOption(new SearchOption(SearchOption.MAXIMUM_RATIO, argParam));
                                    else
                                        validArgs = false;
                                    break;
                                case "-safesearch":
                                case "-ss":
                                    if (bool.TryParse(argParam, out boolSponge))
                                        searchOptions.addOption(new SearchOption(SearchOption.SAFE_SEARCH, argParam));
                                    else
                                        validArgs = false;
                                    break;
                                case "-searchdomain":
                                case "-sd":
                                    searchOptions.addOption(new SearchOption(SearchOption.SEARCH_DOMAIN, argParam));
                                    break;
                                case "-colorsinimage":
                                case "-colors":
                                case "-color":
                                case "-c":
                                    FieldInfo[] colorFields = typeof(ColorsInImage).GetFields();
                                    bool colorFound = false;
                                    foreach (FieldInfo field in colorFields)
                                        if (field.Name.ToLower().Equals(argParam))
                                        {
                                            searchOptions.addOption(new SearchOption(SearchOption.COLORS_IN_IMAGE, field.GetRawConstantValue()));
                                            colorFound = true;
                                            break;
                                        }
                                    if (!colorFound)
                                        validArgs = false;
                                    break;
                                case "-typeofimage":
                                case "-type":
                                case "-t":
                                    FieldInfo[] typeFields = typeof(TypeOfImage).GetFields();
                                    bool typeFound = false;
                                    foreach (FieldInfo field in typeFields)
                                        if (field.Name.ToLower().Equals(argParam))
                                        {
                                            searchOptions.addOption(new SearchOption(SearchOption.TYPE_OF_IMAGE, field.GetRawConstantValue()));
                                            typeFound = true;
                                            break;
                                        }
                                    if (!typeFound)
                                        validArgs = false;
                                    break;
                                default:
                                    if (term == null)
                                        term = arg.Replace("@", "\"");
                                    else
                                        validArgs = false;
                                    break;
                            }
                        }

                        if (validArgs)
                        {
                            Log.info("Starting a quick run.");
                            ExactFerret.runOnce(term, searchOptions);
                        }

                        break;
                    case "-import":
                    case "-i":
                        // Import Exact Ferret settings
                        if (args.Length > 1)
                        {
                            string settingsFilePath = args[1];
                            bool success = PropertiesManager.importSettings(settingsFilePath);
                            if (success)
                            {
                                PropertiesManager.save();
                                MessageBox.Show("The settings were imported successfully.", "Exact Ferret", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                                MessageBox.Show("The settings file you are trying to import is not in the correct format.", "Exact Ferret", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                            validArgs = false;
                        break;
                    case "-panic":
                    case "-p":
                        Log.info("Activating panic mode!");
                        ExactFerret.panic();
                        break;
                    case "-delay":
                    case "-d":
                        // Run the process to change the wallpaper, but delay the first run
                        ExactFerret.startBackgroundProcess(true);
                        break;
                    case "-reset":
                    case "-r":
                        // Resets the keyboard shortcuts so they work (required because of Windows bug)
                        // Wait a little bit, then run -resetnowait, then exit
                        Thread.Sleep(10000);
                        string installDir = PropertiesManager.getInstallationDirectoryPath();
                        Process process = new Process();
                        process.StartInfo.FileName = installDir + "\\Exact Ferret.exe";
                        process.StartInfo.Arguments = "-resetnowait";
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                        break;
                    case "-resetnowait":
                        PropertiesManager.resetShortcuts();
                        break;
                    case "-desktop":
                    case "-k":
                        // Opens the desktop text form to display text on the desktop
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new DesktopTextForm());
                        break;
                    default:
                        validArgs = false;
                        break;
                }

                if (!validArgs)
                {
                    // Display help text
                    Console.WriteLine(Properties.Resources.HELP_TEXT);
                }
            }
            else
            {
                // Run the process to change the wallpaper without delay
                ExactFerret.startBackgroundProcess(false);
            }
        }
    }
}
