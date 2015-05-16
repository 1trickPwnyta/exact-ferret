using Exact_Ferret.Utility_Classes;
using Microsoft.Win32;
using Shell32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret.Settings_Classes
{
    class PropertiesManager
    {
        public const string EXACT_FERRET_ENVIRONMENT_VARIABLE = "%EXACTFERRET%";
        public const string DATA_FOLDER_ENVIRONMENT_VARIABLE = "%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\Exact Ferret\\Data";
        public const string EXACT_FERRET_EXECUTABLE_NAME = "Exact Ferret.exe";
        public const string TRAY_ICON_EXECUTABLE_NAME = "Exact Ferret Tray Icon.exe";
        public const string METADATA_EXECUTABLE_NAME = "JpegMetaData.exe";
        public const string HELP_FILE_PATH = "http://exactferret.kangaroostandard.com/help/";

        private const string REGISTRY_RUN_PATH = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string REGISTRY_RUN_VALUE_NAME = "Exact Ferret";
        private const string REGISTRY_DESKTOP_LABEL_VALUE_NAME = "Exact Ferret Desktop Label";
        private const string START_AT_LOGON_PROPERTY_NAME = "startAtLogon";
        private const string ENABLE_DESKTOP_LABEL_PROPERTY_NAME = "enableDesktopLabel";
        //private const string QUICK_RUN_SHORTCUT_PROPERTY_NAME = "quickRunShortcut";
        //private const string PANIC_SHORTCUT_PROPERTY_NAME = "panicShrotcut";
        //private const string SETTINGS_SHORTCUT_PROPERTY_NAME = "settingsShortcut";

        private const string QUICK_RUN_SHORTCUT_NAME = "quickrun.lnk";
        private const string PANIC_SHORTCUT_NAME = "panic.lnk";
        private const string SETTINGS_SHORTCUT_NAME = "settings.lnk";

        private const string REGISTRY_SOFTWARE_PATH = "Software\\Exact Ferret";
        private const string REGISTRY_SOFTWARE_VERSION_VALUE_NAME = "Version";

        public static bool dirtyFlag;

        private static bool startAtLogonInitial;
        private static bool startAtLogon;
        private static bool enableDesktopLabelInitial;
        private static bool enableDesktopLabel;

        private static string softwareVersion;

        static PropertiesManager()
        {
            // Get the start at logon value from the registry
            string runKeyValue = null;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN_PATH))
            {
                runKeyValue = key.GetValue(REGISTRY_RUN_VALUE_NAME, "").ToString();
                key.Close();
            }
            startAtLogonInitial = runKeyValue != null && runKeyValue != "";
            startAtLogon = startAtLogonInitial;

            // Get the enable desktop label value from the registry
            runKeyValue = null;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN_PATH))
            {
                runKeyValue = key.GetValue(REGISTRY_DESKTOP_LABEL_VALUE_NAME, "").ToString();
                key.Close();
            }
            enableDesktopLabelInitial = runKeyValue != null && runKeyValue != "";
            enableDesktopLabel = enableDesktopLabelInitial;

            // Get the software version from the registry
            string softwareVersionValue = null;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_SOFTWARE_PATH))
            {
                if (key == null)
                    softwareVersionValue = "0";
                else
                {
                    softwareVersionValue = key.GetValue(REGISTRY_SOFTWARE_VERSION_VALUE_NAME, 0).ToString();
                    key.Close();
                }
            }
            softwareVersion = softwareVersionValue != null ? softwareVersionValue : "0";

            // Update the settings if this is a new version
            if (!softwareVersion.Equals(Application.ProductVersion))
            {
                Properties.Settings.Default.Upgrade();
            }

            dirtyFlag = false;
        }

        public static void initialize() { 
            // Invokes the static constructor automatically
        }

        public static void resetShortcuts()
        {
            /*
             * Recreate shortcut keys (because of a bug in Windows that has been around forever and Microsoft has never bothered to 
             * fix it because they don't care about producing quality products - bug makes shortcut keys stop working after logging
             * off Windows)
             */

            // The old way - repeat for each shortcut:
            /*
            // Get the settings shortcut key
            string settingsShortcutLnk = getSettingsShortcutPath();
            settingsShortcutInitial = ShortcutUtil.getShortcutHoyKey(settingsShortcutLnk);
            if (settingsShortcutInitial == -1)
                settingsShortcutInitial = 0;
            if (settingsShortcutInitial > 1600)
                settingsShortcutInitial -= 1600;
            settingsShortcut = settingsShortcutInitial;
             */

            // First remember their current values
            int quickRunShortcut = getQuickRunShortcut();
            int panicShortcut = getPanicShortcut();
            int settingsShortcut = getSettingsShortcut();
            
            // Then reset them to the different key (Z)
            if (quickRunShortcut != 0)
                setQuickRunShortcut(26);
            if (panicShortcut != 0)
                setPanicShortcut(26);
            if (settingsShortcut != 0)
                setSettingsShortcut(26);
            
            // Commit that
            commitQuickRunShortcut();
            commitPanicShortcut();
            commitSettingsShortcut();
            
            // Then restore their values
            setQuickRunShortcut(quickRunShortcut);
            setPanicShortcut(panicShortcut);
            setSettingsShortcut(settingsShortcut);
            
            // And commit that
            commitQuickRunShortcut();
            commitPanicShortcut();
            commitSettingsShortcut();
        }

        private static bool isAllDay(DateTime start, DateTime end)
        {
            return start.Date == end.Date && 
                    start.Hour == 0 && start.Minute == 0 && start.Second == 0 &&
                    end.Hour == 23 && end.Minute == 59 && end.Second == 59;
        }

        private static bool isDisabled(DateTime start, DateTime end)
        {
            return start.TimeOfDay >= end.TimeOfDay;
        }

        public static void save()
        {
            // Save all settings
            Properties.Settings.Default.Save();

            // Save start at logon registry value
            commitStartAtLogon(startAtLogon);

            // Save the enable desktop label registry value
            commitEnableDesktopLabel(enableDesktopLabel);

            // Save the shortcut values
            commitQuickRunShortcut();
            commitPanicShortcut();
            commitSettingsShortcut();

            // Clear the dirty flag
            dirtyFlag = false;
        }

        private static void commitStartAtLogon(bool startAtLogon)
        {
            if (startAtLogon)
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN_PATH, true))
                {
                    string executablePath = "\"" + getInstallationDirectoryPath() + "\\" + TRAY_ICON_EXECUTABLE_NAME + "\"";
                    key.SetValue(REGISTRY_RUN_VALUE_NAME, executablePath);
                    key.Close();
                }
            }
            else
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN_PATH, true))
                {
                    if (key.GetValue(REGISTRY_RUN_VALUE_NAME) != null)
                        key.DeleteValue(REGISTRY_RUN_VALUE_NAME);
                    key.Close();
                }
            }
        }

        private static void commitEnableDesktopLabel(bool enableDesktopLabel)
        {
            if (enableDesktopLabel)
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN_PATH, true))
                {
                    string executablePath = "\"" + getInstallationDirectoryPath() + "\\" + EXACT_FERRET_EXECUTABLE_NAME + "\" -k";
                    key.SetValue(REGISTRY_DESKTOP_LABEL_VALUE_NAME, executablePath);
                    key.Close();
                }
            }
            else
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN_PATH, true))
                {
                    if (key.GetValue(REGISTRY_DESKTOP_LABEL_VALUE_NAME) != null)
                        key.DeleteValue(REGISTRY_DESKTOP_LABEL_VALUE_NAME);
                    key.Close();
                }
            }
        }

        private static void commitQuickRunShortcut()
        {
            int code = getQuickRunShortcut();

            if (code > 0)
            {
                char letter = (char) ('A' + (char) (code - 1));
                ShortcutUtil.createShortcut(getQuickRunShortcutPath(), getInstallationDirectoryPath() + "\\" + EXACT_FERRET_EXECUTABLE_NAME, getInstallationDirectoryPath(), "-quick", "", "Ctrl+Alt+" + letter);
            }
            else
            {
                File.Delete(getQuickRunShortcutPath());
            }
        }

        private static void commitPanicShortcut()
        {
            int code = getPanicShortcut();

            if (code > 0)
            {
                char letter = (char)('A' + (char)(code - 1));
                ShortcutUtil.createShortcut(getPanicShortcutPath(), getInstallationDirectoryPath() + "\\" + EXACT_FERRET_EXECUTABLE_NAME, getInstallationDirectoryPath(), "-panic", "", "Ctrl+Alt+" + letter);
            }
            else
            {
                File.Delete(getPanicShortcutPath());
            }
        }

        private static void commitSettingsShortcut()
        {
            int code = getSettingsShortcut();

            if (code > 0)
            {
                char letter = (char)('A' + (char)(code - 1));
                ShortcutUtil.createShortcut(getSettingsShortcutPath(), getInstallationDirectoryPath() + "\\" + EXACT_FERRET_EXECUTABLE_NAME, getInstallationDirectoryPath(), "-settings", "", "Ctrl+Alt+" + letter);
            }
            else
            {
                File.Delete(getSettingsShortcutPath());
            }
        }

        private static void commitSoftwareVersion(string version)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_SOFTWARE_PATH, true);
            if (key == null) {
                Registry.CurrentUser.CreateSubKey(REGISTRY_SOFTWARE_PATH);
                key = Registry.CurrentUser.OpenSubKey(REGISTRY_SOFTWARE_PATH, true);
            }
            key.SetValue(REGISTRY_SOFTWARE_VERSION_VALUE_NAME, version);
            key.Close();
        }

        public static void restore()
        {
            // Restore all settings
            Properties.Settings.Default.Reload();

            // Restore start at logon registry value
            commitStartAtLogon(startAtLogonInitial);

            // Restore enable desktop label registry value
            commitEnableDesktopLabel(enableDesktopLabelInitial);

            // Restore the shortcut keys
            commitQuickRunShortcut();
            commitPanicShortcut();
            commitSettingsShortcut();

            // Clear the dirty flag
            dirtyFlag = false;
        }

        public static void exportSettings(string filePath)
        {
            exportSettings(filePath, null);
        }

        public static void exportSettings(string filePath, string[] propertyNames)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                if (propertyNames == null)
                {
                    SettingsPropertyCollection collection = Properties.Settings.Default.Properties;
                    foreach (SettingsProperty property in collection)
                    {
                        writeSetting(property.Name, property.PropertyType, writer);
                    }

                    // Start at logon
                    writer.WriteLine(START_AT_LOGON_PROPERTY_NAME + ":" + Base64Util.Base64Encode(startAtLogon.ToString()));

                    // Enable desktop label
                    writer.WriteLine(ENABLE_DESKTOP_LABEL_PROPERTY_NAME + ":" + Base64Util.Base64Encode(enableDesktopLabel.ToString()));

                    // Shortcuts - don't need to do now since they are bound to settings
                    /*
                    writer.WriteLine(QUICK_RUN_SHORTCUT_PROPERTY_NAME + ":" + Base64Util.Base64Encode(quickRunShortcut.ToString()));
                    writer.WriteLine(PANIC_SHORTCUT_PROPERTY_NAME + ":" + Base64Util.Base64Encode(panicShortcut.ToString()));
                    writer.WriteLine(SETTINGS_SHORTCUT_PROPERTY_NAME + ":" + Base64Util.Base64Encode(settingsShortcut.ToString()));
                     */
                }
                else
                {
                    foreach (string propertyName in propertyNames)
                    {
                        // Start at logon
                        if (propertyName.Equals(START_AT_LOGON_PROPERTY_NAME))
                        {
                            writer.WriteLine(START_AT_LOGON_PROPERTY_NAME + ":" + Base64Util.Base64Encode(startAtLogon.ToString()));
                            continue;
                        }

                        // Enable desktop label
                        if (propertyName.Equals(ENABLE_DESKTOP_LABEL_PROPERTY_NAME))
                        {
                            writer.WriteLine(ENABLE_DESKTOP_LABEL_PROPERTY_NAME + ":" + Base64Util.Base64Encode(enableDesktopLabel.ToString()));
                            continue;
                        }

                        // Shortcuts - don't need to do now since they are bound to settings
                        /*
                        if (propertyName.Equals(QUICK_RUN_SHORTCUT_PROPERTY_NAME))
                        {
                            writer.WriteLine(QUICK_RUN_SHORTCUT_PROPERTY_NAME + ":" + Base64Util.Base64Encode(quickRunShortcut.ToString()));
                            continue;
                        }
                        if (propertyName.Equals(PANIC_SHORTCUT_PROPERTY_NAME))
                        {
                            writer.WriteLine(PANIC_SHORTCUT_PROPERTY_NAME + ":" + Base64Util.Base64Encode(panicShortcut.ToString()));
                            continue;
                        }
                        if (propertyName.Equals(SETTINGS_SHORTCUT_PROPERTY_NAME))
                        {
                            writer.WriteLine(SETTINGS_SHORTCUT_PROPERTY_NAME + ":" + Base64Util.Base64Encode(settingsShortcut.ToString()));
                            continue;
                        }*/

                        SettingsProperty property = Properties.Settings.Default.Properties[propertyName];
                        writeSetting(propertyName, property.PropertyType, writer);
                    }
                }
            }
        }

        private static bool writeSetting(string propertyName, Type propertyType, StreamWriter writer)
        {
            if (propertyName.Equals("DO_NOT_OVERWRITE_BLOCKED_PICTURES"))
                return false;

            if (propertyName.Equals("SETTINGS_SCHEDULE"))
                return false;

            if (propertyType == typeof(StringCollection))
            {
                string collectionLine = "";
                foreach (string s in (StringCollection)Properties.Settings.Default[propertyName])
                {
                    string b64 = Base64Util.Base64Encode(s);
                    collectionLine += b64 + ",";
                }
                writer.WriteLine(propertyName + ":" + collectionLine);
            }
            else if (propertyType == typeof(DateTime))
            {
                writer.WriteLine(propertyName + ":" + Base64Util.Base64Encode(
                        ((DateTime)Properties.Settings.Default[propertyName]).Ticks.ToString()));
            }
            else
            {
                writer.WriteLine(propertyName + ":" + Base64Util.Base64Encode(
                        Properties.Settings.Default[propertyName].ToString()));
            }

            return true;
        }

        public static bool importSettings(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (reader.Peek() != -1)
                    {
                        string line = reader.ReadLine();
                        string[] lineParts = line.Split(':');
                        string propertyName = lineParts[0];
                        string propertyValue = lineParts.Length > 1? lineParts[1]: "";

                        if (propertyName.Equals(START_AT_LOGON_PROPERTY_NAME)) {
                            startAtLogon = bool.Parse(Base64Util.Base64Decode(propertyValue));
                            continue;
                        }

                        if (propertyName.Equals(ENABLE_DESKTOP_LABEL_PROPERTY_NAME))
                        {
                            enableDesktopLabel = bool.Parse(Base64Util.Base64Decode(propertyValue));
                            if (enableDesktopLabel && !Communication.ping(1))
                                Communication.startExactFerret(1);

                            continue;
                        }

                        // Don't need to do this anymore because shortcut keys are bound to settings
                        /*
                        if (propertyName.Equals(QUICK_RUN_SHORTCUT_PROPERTY_NAME))
                        {
                            quickRunShortcut = int.Parse(Base64Util.Base64Decode(propertyValue));
                            continue;
                        }
                        if (propertyName.Equals(PANIC_SHORTCUT_PROPERTY_NAME))
                        {
                            panicShortcut = int.Parse(Base64Util.Base64Decode(propertyValue));
                            continue;
                        }
                        if (propertyName.Equals(SETTINGS_SHORTCUT_PROPERTY_NAME))
                        {
                            settingsShortcut = int.Parse(Base64Util.Base64Decode(propertyValue));
                            continue;
                        }*/

                        SettingsProperty property = Properties.Settings.Default.Properties[propertyName];
                        Type propertyType = property.PropertyType;
                        if (propertyType == typeof(int))
                        {
                            Properties.Settings.Default[propertyName] = int.Parse(Base64Util.Base64Decode(propertyValue));
                        }
                        else if (propertyType == typeof(decimal))
                        {
                            Properties.Settings.Default[propertyName] = decimal.Parse(Base64Util.Base64Decode(propertyValue));
                        }
                        else if (propertyType == typeof(bool))
                        {
                            if (propertyName.Equals("DO_NOT_OVERWRITE_BLOCKED_PICTURES"))
                                continue;

                            Properties.Settings.Default[propertyName] = bool.Parse(Base64Util.Base64Decode(propertyValue));
                        }
                        else if (propertyType == typeof(string))
                        {
                            if (propertyName.Equals("SETTINGS_SCHEDULE"))
                                continue;

                            Properties.Settings.Default[propertyName] = Base64Util.Base64Decode(propertyValue);
                        }
                        else if (propertyType == typeof(StringCollection))
                        {
                            if ((propertyName.Equals("BLOCKED_IMAGES") || propertyName.Equals("BLOCKED_DOMAINS")) &&
                                    getDoNotOverwriteBlockedPicturesEnabled())
                                continue;

                            StringCollection collection = (StringCollection)Properties.Settings.Default[propertyName];
                            collection.Clear();

                            string[] values = propertyValue.Split(',');
                            foreach (string b64 in values)
                            {
                                if (b64.Length > 0)
                                {
                                    string value = Base64Util.Base64Decode(b64);
                                    collection.Add(value);
                                }
                            }
                        }
                        else if (propertyType == typeof(DateTime))
                        {
                            Properties.Settings.Default[propertyName] = new DateTime(long.Parse(Base64Util.Base64Decode(propertyValue)));
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static string getInstallationDirectoryPath()
        {
            return Environment.ExpandEnvironmentVariables(EXACT_FERRET_ENVIRONMENT_VARIABLE);
        }

        public static DirectoryInfo getInstallationDirectory()
        {
            return new DirectoryInfo(Environment.ExpandEnvironmentVariables(EXACT_FERRET_ENVIRONMENT_VARIABLE));
        }

        public static bool getAutoUpdateEnabled()
        {
            return Properties.Settings.Default.AUTO_UPDATE;
        }

        public static void setAutoUpdate(bool enabled)
        {
            Properties.Settings.Default.AUTO_UPDATE = enabled;
            dirtyFlag = true;
        }

        public static bool getStartAtLogonEnabled()
        {
            return startAtLogon;
        }

        public static void setStartAtLogin(bool enabled)
        {
            startAtLogon = enabled;
            dirtyFlag = true;
        }

        public static bool getEnableDesktopLabel()
        {
            return enableDesktopLabel;
        }

        public static void setEnableDesktopLabel(bool enabled)
        {
            enableDesktopLabel = enabled;
            dirtyFlag = true;
        }

        public static int getQuickRunShortcut()
        {
            return Properties.Settings.Default.QUICK_RUN_SHORTCUT;
        }

        public static void setQuickRunShortcut(int code)
        {
            Properties.Settings.Default.QUICK_RUN_SHORTCUT = code;
            dirtyFlag = true;
        }

        public static int getPanicShortcut()
        {
            return Properties.Settings.Default.PANIC_SHORTCUT;
        }

        public static void setPanicShortcut(int code)
        {
            Properties.Settings.Default.PANIC_SHORTCUT = code;
            dirtyFlag = true;
        }

        public static int getSettingsShortcut()
        {
            return Properties.Settings.Default.SETTINGS_SHORTCUT;
        }

        public static void setSettingsShortcut(int code)
        {
            Properties.Settings.Default.SETTINGS_SHORTCUT = code;
            dirtyFlag = true;
        }

        public static string getSoftwareVersion()
        {
            return softwareVersion;
        }

        public static string getSoftwareMinorVersion()
        {
            string[] versionNumbers = softwareVersion.Split('.');
            return versionNumbers[0] + "." + versionNumbers[1];
        }

        public static void setSoftwareVersion(string version)
        {
            softwareVersion = version;
            commitSoftwareVersion(version);
        }

        public static int getRunIntervalMinutesComponent()
        {
            return (int)Properties.Settings.Default.INTERVAL_MINUTES;
        }

        public static int getRunIntervalSecondsComponent()
        {
            return (int)Properties.Settings.Default.INTERVAL_SECONDS;
        }

        public static TimeSpan getRunIntervalTimeSpan()
        {
            return new TimeSpan(0, (int) Properties.Settings.Default.INTERVAL_MINUTES, 
                    (int) Properties.Settings.Default.INTERVAL_SECONDS);
        }

        public static void setRunIntervalMinutesComponent(int minutes)
        {
            Properties.Settings.Default.INTERVAL_MINUTES = minutes;
            dirtyFlag = true;
        }

        public static void setRunIntervalSecondsComponent(int seconds)
        {
            Properties.Settings.Default.INTERVAL_SECONDS = seconds;
            dirtyFlag = true;
        }

        public static void setRunInterval(int minutes, int seconds)
        {
            Properties.Settings.Default.INTERVAL_MINUTES = minutes;
            Properties.Settings.Default.INTERVAL_SECONDS = seconds;
            dirtyFlag = true;
        }

        public static void setRunInterval(TimeSpan interval)
        {
            int minutes = (int) interval.TotalMinutes;
            int seconds = interval.Seconds;
            Properties.Settings.Default.INTERVAL_MINUTES = minutes;
            Properties.Settings.Default.INTERVAL_SECONDS = seconds;
            dirtyFlag = true;
        }

        public static string getLogFilePath()
        {
            return Environment.ExpandEnvironmentVariables(Properties.Settings.Default.LOG_FILE_LOCATION);
        }

        public static FileInfo getLogFile()
        {
            return new FileInfo(Properties.Settings.Default.LOG_FILE_LOCATION);
        }

        public static string getLogFolderPath()
        {
            FileInfo i = new FileInfo(Environment.ExpandEnvironmentVariables(Properties.Settings.Default.LOG_FILE_LOCATION));
            return i.Directory.FullName;
        }

        public static DirectoryInfo getLogFolder()
        {
            FileInfo i = new FileInfo(Properties.Settings.Default.LOG_FILE_LOCATION);
            return i.Directory;
        }

        public static void setLogFilePath(string path)
        {
            Properties.Settings.Default.LOG_FILE_LOCATION = path;
            dirtyFlag = true;
        }

        public static void setLogFile(FileInfo info)
        {
            Properties.Settings.Default.LOG_FILE_LOCATION = info.FullName;
            dirtyFlag = true;
        }

        public static int getLogRolloverKilobytes()
        {
            return (int) Properties.Settings.Default.LOG_ROLLOVER_KBYTES;
        }

        public static int getLogRolloverBytes()
        {
            return (int) Properties.Settings.Default.LOG_ROLLOVER_KBYTES * 1024;
        }

        public static void setLogRolloverKilobytes(int kilobytes)
        {
            Properties.Settings.Default.LOG_ROLLOVER_KBYTES = kilobytes;
            dirtyFlag = true;
        }

        public static void setLogRolloverBytes(int bytes)
        {
            Properties.Settings.Default.LOG_ROLLOVER_KBYTES = bytes / 1024;
            dirtyFlag = true;
        }

        public static int getLogLevel()
        {
            return Properties.Settings.Default.LOG_LEVEL;
        }

        public static void setLogLevel(int level)
        {
            Properties.Settings.Default.LOG_LEVEL = level;
            dirtyFlag = true;
        }

        private static void createDataPath()
        {
            string dataFolder = Environment.ExpandEnvironmentVariables(DATA_FOLDER_ENVIRONMENT_VARIABLE);
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder).Attributes = FileAttributes.Directory | FileAttributes.Hidden;

                // Initialize the default panic shortcut (P)
                setPanicShortcut(16);
                save();
            }
        }

        public static string getQuickRunShortcutPath()
        {
            createDataPath();
            return Environment.ExpandEnvironmentVariables(DATA_FOLDER_ENVIRONMENT_VARIABLE) + "\\" + QUICK_RUN_SHORTCUT_NAME;
        }

        public static string getPanicShortcutPath()
        {
            createDataPath();
            return Environment.ExpandEnvironmentVariables(DATA_FOLDER_ENVIRONMENT_VARIABLE) + "\\" + PANIC_SHORTCUT_NAME;
        }

        public static string getSettingsShortcutPath()
        {
            createDataPath();
            return Environment.ExpandEnvironmentVariables(DATA_FOLDER_ENVIRONMENT_VARIABLE) + "\\" + SETTINGS_SHORTCUT_NAME;
        }

        public static string getPictureFolderPath()
        {
            return Environment.ExpandEnvironmentVariables(Properties.Settings.Default.IMAGE_LOCATION);
        }

        public static DirectoryInfo getPictureFolder()
        {
            return new DirectoryInfo(Properties.Settings.Default.IMAGE_LOCATION);
        }

        public static void setPictureFolderPath(string path)
        {
            Properties.Settings.Default.IMAGE_LOCATION = path;
            dirtyFlag = true;
        }

        public static void setPictureFolder(DirectoryInfo info)
        {
            Properties.Settings.Default.IMAGE_LOCATION = info.FullName;
            dirtyFlag = true;
        }

        public static bool getChangeWallpaperEnabled()
        {
            return Properties.Settings.Default.CHANGE_WALLPAPER;
        }

        public static void setChangeWallpaper(bool enabled)
        {
            Properties.Settings.Default.CHANGE_WALLPAPER = enabled;
            dirtyFlag = true;
        }

        public static bool getChangeLockScreenEnabled()
        {
            return Properties.Settings.Default.CHANGE_LOCK_SCREEN;
        }

        public static void setChangeLockScreen(bool enabled)
        {
            Properties.Settings.Default.CHANGE_LOCK_SCREEN = enabled;
            dirtyFlag = true;
        }

        public static int getMaxOldImageFiles()
        {
            return (int) Properties.Settings.Default.MAX_OLD_IMAGES;
        }

        public static void setMaxOldImages(int max)
        {
            Properties.Settings.Default.MAX_OLD_IMAGES = max;
            dirtyFlag = true;
        }

        public static int getMinimumImageWidth()
        {
            return (int)Properties.Settings.Default.MINIMUM_IMAGE_WIDTH;
        }

        public static int getMinimumImageHeight()
        {
            return (int)Properties.Settings.Default.MINIMUM_IMAGE_HEIGHT;
        }

        public static Size getMinimumImageSize()
        {
            return new Size((int) Properties.Settings.Default.MINIMUM_IMAGE_WIDTH, 
                    (int) Properties.Settings.Default.MINIMUM_IMAGE_HEIGHT);
        }

        public static void setMinimumImageWidth(int width)
        {
            Properties.Settings.Default.MINIMUM_IMAGE_WIDTH = width;
            dirtyFlag = true;
        }

        public static void setMinimumImageHeight(int height)
        {
            Properties.Settings.Default.MINIMUM_IMAGE_HEIGHT = height;
            dirtyFlag = true;
        }

        public static void setMinimumImageSize(int width, int height)
        {
            Properties.Settings.Default.MINIMUM_IMAGE_WIDTH = width;
            Properties.Settings.Default.MINIMUM_IMAGE_HEIGHT = height;
            dirtyFlag = true;
        }

        public static void setMinimumImageSize(Size size)
        {
            Properties.Settings.Default.MINIMUM_IMAGE_WIDTH = size.Width;
            Properties.Settings.Default.MINIMUM_IMAGE_HEIGHT = size.Height;
            dirtyFlag = true;
        }

        public static int getMaximumImageWidth()
        {
            return (int) Properties.Settings.Default.MAXIMUM_IMAGE_WIDTH;
        }

        public static int getMaximumImageHeight()
        {
            return (int)Properties.Settings.Default.MAXIMUM_IMAGE_HEIGHT;
        }

        public static Size getMaximumImageSize()
        {
            return new Size((int)Properties.Settings.Default.MAXIMUM_IMAGE_WIDTH,
                    (int)Properties.Settings.Default.MAXIMUM_IMAGE_HEIGHT);
        }

        public static void setMaximumImageWidth(int width)
        {
            Properties.Settings.Default.MAXIMUM_IMAGE_WIDTH = width;
            dirtyFlag = true;
        }

        public static void setMaximumImageHeight(int height)
        {
            Properties.Settings.Default.MAXIMUM_IMAGE_HEIGHT = height;
            dirtyFlag = true;
        }

        public static void setMaximumImageSize(int width, int height)
        {
            Properties.Settings.Default.MAXIMUM_IMAGE_WIDTH = width;
            Properties.Settings.Default.MAXIMUM_IMAGE_HEIGHT = height;
            dirtyFlag = true;
        }

        public static void setMaximumImageSize(Size size)
        {
            Properties.Settings.Default.MAXIMUM_IMAGE_WIDTH = size.Width;
            Properties.Settings.Default.MAXIMUM_IMAGE_HEIGHT = size.Height;
            dirtyFlag = true;
        }

        public static void setImageWidthRange(int min, int max)
        {
            Properties.Settings.Default.MINIMUM_IMAGE_WIDTH = min;
            Properties.Settings.Default.MAXIMUM_IMAGE_WIDTH = max;
            dirtyFlag = true;
        }

        public static void setImageHeightRange(int min, int max)
        {
            Properties.Settings.Default.MINIMUM_IMAGE_HEIGHT = min;
            Properties.Settings.Default.MAXIMUM_IMAGE_HEIGHT = max;
            dirtyFlag = true;
        }

        public static void setImageSizeRange(int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            Properties.Settings.Default.MINIMUM_IMAGE_WIDTH = minWidth;
            Properties.Settings.Default.MAXIMUM_IMAGE_WIDTH = maxWidth;
            Properties.Settings.Default.MINIMUM_IMAGE_HEIGHT = minHeight;
            Properties.Settings.Default.MAXIMUM_IMAGE_HEIGHT = maxHeight;
            dirtyFlag = true;
        }

        public static void setImageSizeRange(Size min, Size max)
        {
            Properties.Settings.Default.MINIMUM_IMAGE_WIDTH = min.Width;
            Properties.Settings.Default.MAXIMUM_IMAGE_WIDTH = max.Width;
            Properties.Settings.Default.MINIMUM_IMAGE_HEIGHT = min.Height;
            Properties.Settings.Default.MAXIMUM_IMAGE_HEIGHT = max.Height;
            dirtyFlag = true;
        }

        public static double getMinimumImageWidthHeightRatio()
        {
            return (double) Properties.Settings.Default.MINIMUM_IMAGE_RATIO;
        }

        public static void setMinimumImageWidthHeightRatio(double ratio)
        {
            Properties.Settings.Default.MINIMUM_IMAGE_RATIO = (decimal) ratio;
            dirtyFlag = true;
        }

        public static double getMaximumImageWidthHeightRatio()
        {
            return (double)Properties.Settings.Default.MAXIMUM_IMAGE_RATIO;
        }

        public static void setMaximumImageWidthHeightRatio(double ratio)
        {
            Properties.Settings.Default.MAXIMUM_IMAGE_RATIO = (decimal)ratio;
            dirtyFlag = true;
        }

        public static void setImageWidthHeightRatioRange(double min, double max)
        {
            Properties.Settings.Default.MINIMUM_IMAGE_RATIO = (decimal)min;
            Properties.Settings.Default.MAXIMUM_IMAGE_RATIO = (decimal)max;
            dirtyFlag = true;
        }

        public static string getSearchTermModifier()
        {
            return Properties.Settings.Default.SEARCH_TERM_MODIFIER;
        }

        public static void setSearchTermModifier(string modifier)
        {
            Properties.Settings.Default.SEARCH_TERM_MODIFIER = modifier;
            dirtyFlag = true;
        }

        public static int getDownloadAttemptLimit()
        {
            return (int)Properties.Settings.Default.DOWNLOAD_ATTEMPTS;
        }

        public static void setDownloadAttemptLimit(int limit)
        {
            Properties.Settings.Default.DOWNLOAD_ATTEMPTS = limit;
            dirtyFlag = true;
        }

        public static bool getSafeSearchEnabled()
        {
            return Properties.Settings.Default.SAFE_SEARCH;
        }

        public static void setSafeSearch(bool enabled)
        {
            Properties.Settings.Default.SAFE_SEARCH = enabled;
            dirtyFlag = true;
        }

        public static int getColorsInImage()
        {
            return Properties.Settings.Default.COLORS_IN_IMAGE;
        }

        public static void setColorsInImage(int colors)
        {
            Properties.Settings.Default.COLORS_IN_IMAGE = colors;
            dirtyFlag = true;
        }

        public static int getTypeOfImage()
        {
            return Properties.Settings.Default.TYPE_OF_IMAGE;
        }

        public static void setTypeOfImage(int type)
        {
            Properties.Settings.Default.TYPE_OF_IMAGE = type;
            dirtyFlag = true;
        }

        public static string getSearchDomain()
        {
            return Properties.Settings.Default.SEARCH_DOMAIN;
        }

        public static void setSearchDomain(string domain)
        {
            Properties.Settings.Default.SEARCH_DOMAIN = domain;
            dirtyFlag = true;
        }

        public static int getWallpaperStyle()
        {
            return Properties.Settings.Default.WALLPAPER_STYLE;
        }

        public static void setWallpaperStyle(int style)
        {
            Properties.Settings.Default.WALLPAPER_STYLE = style;
            dirtyFlag = true;
        }

        public static string getDictionaryPath()
        {
            return Environment.ExpandEnvironmentVariables(Properties.Settings.Default.DICTIONARY_FILE);
        }

        public static FileInfo getDictionary()
        {
            return new FileInfo(Properties.Settings.Default.DICTIONARY_FILE);
        }

        public static void setDictionaryPath(string path)
        {
            Properties.Settings.Default.DICTIONARY_FILE = path;
            dirtyFlag = true;
        }

        public static void setDictionary(FileInfo info)
        {
            Properties.Settings.Default.DICTIONARY_FILE = info.FullName;
            dirtyFlag = true;
        }

        public static int getHttpRequestTimeoutSeconds()
        {
            return (int) Properties.Settings.Default.HTTP_REQUEST_TIMEOUT;
        }

        public static void setHttpRequestTimeoutSeconds(int seconds)
        {
            Properties.Settings.Default.HTTP_REQUEST_TIMEOUT = seconds;
            dirtyFlag = true;
        }

        public static int getLogRetentionDays()
        {
            return (int)Properties.Settings.Default.LOG_RETENTION_DAYS;
        }

        public static void setLogRetentionDays(int days)
        {
            Properties.Settings.Default.LOG_RETENTION_DAYS = days;
            dirtyFlag = true;
        }

        public static StringCollection getBlockedImagesStringCollection()
        {
            return Properties.Settings.Default.BLOCKED_IMAGES;
        }

        public static object getBlockedImagesDataSource()
        {
            return Properties.Settings.Default.BLOCKED_IMAGES;
        }

        public static object getBlockedDomainsDataSource()
        {
            return Properties.Settings.Default.BLOCKED_DOMAINS;
        }

        public static object getSearchEngineOrderDataSource()
        {
            return Properties.Settings.Default.SEARCH_ENGINE_ORDER;
        }

        public static StringCollection getBlockedDomainsStringCollection()
        {
            return Properties.Settings.Default.BLOCKED_DOMAINS;
        }

        public static void blockImage(string imageUrl)
        {
            if (imageUrl.Contains("://"))
                imageUrl = imageUrl.Substring(imageUrl.IndexOf("://") + 3);

            if (!Properties.Settings.Default.BLOCKED_IMAGES.Contains(imageUrl))
                Properties.Settings.Default.BLOCKED_IMAGES.Add(imageUrl);
            ArrayList.Adapter(Properties.Settings.Default.BLOCKED_IMAGES).Sort();
            dirtyFlag = true;
        }

        public static void blockDomain(string domain)
        {
            if (!Properties.Settings.Default.BLOCKED_DOMAINS.Contains(domain))
                Properties.Settings.Default.BLOCKED_DOMAINS.Add(domain);
            ArrayList.Adapter(Properties.Settings.Default.BLOCKED_DOMAINS).Sort();
            dirtyFlag = true;
        }

        public static void unblockImages(IEnumerable<string> urlsToUnblock)
        {
            foreach (string imageUrl in urlsToUnblock)
            {
                Properties.Settings.Default.BLOCKED_IMAGES.Remove(imageUrl);
            }
            dirtyFlag = true;
        }

        public static void unblockDomains(IEnumerable<string> domainsToUnblock)
        {
            foreach (string domain in domainsToUnblock)
            {
                Properties.Settings.Default.BLOCKED_DOMAINS.Remove(domain);
            }
            dirtyFlag = true;
        }

        public static StringCollection getSearchEngineOrder()
        {
            return Properties.Settings.Default.SEARCH_ENGINE_ORDER;
        }

        public static void setSearchEngineOrder(StringCollection searchEngines)
        {
            Properties.Settings.Default.SEARCH_ENGINE_ORDER = searchEngines;
            dirtyFlag = true;
        }

        public static string getGoogleApiKey()
        {
            return Properties.Settings.Default.GOOGLE_API_KEY;
        }

        public static void setGoogleApiKey(string key)
        {
            Properties.Settings.Default.GOOGLE_API_KEY = key;
            dirtyFlag = true;
        }

        public static string getBingAccessToken()
        {
            return Properties.Settings.Default.BING_ACCESS_TOKEN;
        }

        public static void setBingAccessToken(string token)
        {
            Properties.Settings.Default.BING_ACCESS_TOKEN = token;
            dirtyFlag = true;
        }

        public static bool getApiLimitWarningEnabled()
        {
            return Properties.Settings.Default.API_LIMIT_WARNING;
        }

        public static void setApiLimitWarning(bool enabled)
        {
            Properties.Settings.Default.API_LIMIT_WARNING = enabled;
            dirtyFlag = true;
        }

        public static bool getSingleApiLimitWarningEnabled()
        {
            return Properties.Settings.Default.SINGLE_API_LIMIT_WARNING;
        }

        public static void setSingleApiLimitWarningEnabled(bool enabled)
        {
            Properties.Settings.Default.SINGLE_API_LIMIT_WARNING = enabled;
            dirtyFlag = true;
        }

        public static bool getCycleAtApiLimitEnabled()
        {
            return Properties.Settings.Default.CYCLE_AT_API_LIMIT;
        }

        public static void setCycleAtApiLimit(bool enabled)
        {
            Properties.Settings.Default.CYCLE_AT_API_LIMIT = enabled;
            dirtyFlag = true;
        }

        public static DateTime getSundayScheduleStart()
        {
            return Properties.Settings.Default.SUNDAY_START;
        }

        public static DateTime getSundayScheduleStop()
        {
            return Properties.Settings.Default.SUNDAY_STOP;
        }

        public static bool getSundayScheduleAllDayEnabled()
        {
            return isAllDay(Properties.Settings.Default.SUNDAY_START, Properties.Settings.Default.SUNDAY_STOP);
        }

        public static bool getSundayScheduleDisabled()
        {
            return isDisabled(Properties.Settings.Default.SUNDAY_START, Properties.Settings.Default.SUNDAY_STOP);
        }

        public static void setSundayScheduleStart(DateTime time)
        {
            Properties.Settings.Default.SUNDAY_START = time;
            dirtyFlag = true;
        }

        public static void setSundayScheduleStop(DateTime time)
        {
            Properties.Settings.Default.SUNDAY_STOP = time;
            dirtyFlag = true;
        }

        public static void setSundayScheduleAllDay()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(1).AddSeconds(-1);
            Properties.Settings.Default.SUNDAY_START = start;
            Properties.Settings.Default.SUNDAY_STOP = end;
            dirtyFlag = true;
        }

        public static void setSundayScheduleDisabled()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today;
            Properties.Settings.Default.SUNDAY_START = start;
            Properties.Settings.Default.SUNDAY_STOP = end;
            dirtyFlag = true;
        }

        public static DateTime getMondayScheduleStart()
        {
            return Properties.Settings.Default.MONDAY_START;
        }

        public static DateTime getMondayScheduleStop()
        {
            return Properties.Settings.Default.MONDAY_STOP;
        }

        public static bool getMondayScheduleAllDayEnabled()
        {
            return isAllDay(Properties.Settings.Default.MONDAY_START, Properties.Settings.Default.MONDAY_STOP);
        }

        public static bool getMondayScheduleDisabled()
        {
            return isDisabled(Properties.Settings.Default.MONDAY_START, Properties.Settings.Default.MONDAY_STOP);
        }

        public static void setMondayScheduleStart(DateTime time)
        {
            Properties.Settings.Default.MONDAY_START = time;
            dirtyFlag = true;
        }

        public static void setMondayScheduleStop(DateTime time)
        {
            Properties.Settings.Default.MONDAY_STOP = time;
            dirtyFlag = true;
        }

        public static void setMondayScheduleAllDay()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(1).AddSeconds(-1);
            Properties.Settings.Default.MONDAY_START = start;
            Properties.Settings.Default.MONDAY_STOP = end;
            dirtyFlag = true;
        }

        public static void setMondayScheduleDisabled()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today;
            Properties.Settings.Default.MONDAY_START = start;
            Properties.Settings.Default.MONDAY_STOP = end;
            dirtyFlag = true;
        }

        public static DateTime getTuesdayScheduleStart()
        {
            return Properties.Settings.Default.TUESDAY_START;
        }

        public static DateTime getTuesdayScheduleStop()
        {
            return Properties.Settings.Default.TUESDAY_STOP;
        }

        public static bool getTuesdayScheduleAllDayEnabled()
        {
            return isAllDay(Properties.Settings.Default.TUESDAY_START, Properties.Settings.Default.TUESDAY_STOP);
        }

        public static bool getTuesdayScheduleDisabled()
        {
            return isDisabled(Properties.Settings.Default.TUESDAY_START, Properties.Settings.Default.TUESDAY_STOP);
        }

        public static void setTuesdayScheduleStart(DateTime time)
        {
            Properties.Settings.Default.TUESDAY_START = time;
            dirtyFlag = true;
        }

        public static void setTuesdayScheduleStop(DateTime time)
        {
            Properties.Settings.Default.TUESDAY_STOP = time;
            dirtyFlag = true;
        }

        public static void setTuesdayScheduleAllDay()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(1).AddSeconds(-1);
            Properties.Settings.Default.TUESDAY_START = start;
            Properties.Settings.Default.TUESDAY_STOP = end;
            dirtyFlag = true;
        }

        public static void setTuesdayScheduleDisabled()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today;
            Properties.Settings.Default.TUESDAY_START = start;
            Properties.Settings.Default.TUESDAY_STOP = end;
            dirtyFlag = true;
        }

        public static DateTime getWednesdayScheduleStart()
        {
            return Properties.Settings.Default.WEDNESDAY_START;
        }

        public static DateTime getWednesdayScheduleStop()
        {
            return Properties.Settings.Default.WEDNESDAY_STOP;
        }

        public static bool getWednesdayScheduleAllDayEnabled()
        {
            return isAllDay(Properties.Settings.Default.WEDNESDAY_START, Properties.Settings.Default.WEDNESDAY_STOP);
        }

        public static bool getWednesdayScheduleDisabled()
        {
            return isDisabled(Properties.Settings.Default.WEDNESDAY_START, Properties.Settings.Default.WEDNESDAY_STOP);
        }

        public static void setWednesdayScheduleStart(DateTime time)
        {
            Properties.Settings.Default.WEDNESDAY_START = time;
            dirtyFlag = true;
        }

        public static void setWednesdayScheduleStop(DateTime time)
        {
            Properties.Settings.Default.WEDNESDAY_STOP = time;
            dirtyFlag = true;
        }

        public static void setWednesdayScheduleAllDay()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(1).AddSeconds(-1);
            Properties.Settings.Default.WEDNESDAY_START = start;
            Properties.Settings.Default.WEDNESDAY_STOP = end;
            dirtyFlag = true;
        }

        public static void setWednesdayScheduleDisabled()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today;
            Properties.Settings.Default.WEDNESDAY_START = start;
            Properties.Settings.Default.WEDNESDAY_STOP = end;
            dirtyFlag = true;
        }

        public static DateTime getThursdayScheduleStart()
        {
            return Properties.Settings.Default.THURSDAY_START;
        }

        public static DateTime getThursdayScheduleStop()
        {
            return Properties.Settings.Default.THURSDAY_STOP;
        }

        public static bool getThursdayScheduleAllDayEnabled()
        {
            return isAllDay(Properties.Settings.Default.THURSDAY_START, Properties.Settings.Default.THURSDAY_STOP);
        }

        public static bool getThursdayScheduleDisabled()
        {
            return isDisabled(Properties.Settings.Default.THURSDAY_START, Properties.Settings.Default.THURSDAY_STOP);
        }

        public static void setThursdayScheduleStart(DateTime time)
        {
            Properties.Settings.Default.THURSDAY_START = time;
            dirtyFlag = true;
        }

        public static void setThursdayScheduleStop(DateTime time)
        {
            Properties.Settings.Default.THURSDAY_STOP = time;
            dirtyFlag = true;
        }

        public static void setThursdayScheduleAllDay()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(1).AddSeconds(-1);
            Properties.Settings.Default.THURSDAY_START = start;
            Properties.Settings.Default.THURSDAY_STOP = end;
            dirtyFlag = true;
        }

        public static void setThursdayScheduleDisabled()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today;
            Properties.Settings.Default.THURSDAY_START = start;
            Properties.Settings.Default.THURSDAY_STOP = end;
            dirtyFlag = true;
        }

        public static DateTime getFridayScheduleStart()
        {
            return Properties.Settings.Default.FRIDAY_START;
        }

        public static DateTime getFridayScheduleStop()
        {
            return Properties.Settings.Default.FRIDAY_STOP;
        }

        public static bool getFridayScheduleAllDayEnabled()
        {
            return isAllDay(Properties.Settings.Default.FRIDAY_START, Properties.Settings.Default.FRIDAY_STOP);
        }

        public static bool getFridayScheduleDisabled()
        {
            return isDisabled(Properties.Settings.Default.FRIDAY_START, Properties.Settings.Default.FRIDAY_STOP);
        }

        public static void setFridayScheduleStart(DateTime time)
        {
            Properties.Settings.Default.FRIDAY_START = time;
            dirtyFlag = true;
        }

        public static void setFridayScheduleStop(DateTime time)
        {
            Properties.Settings.Default.FRIDAY_STOP = time;
            dirtyFlag = true;
        }

        public static void setFridayScheduleAllDay()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(1).AddSeconds(-1);
            Properties.Settings.Default.FRIDAY_START = start;
            Properties.Settings.Default.FRIDAY_STOP = end;
            dirtyFlag = true;
        }

        public static void setFridayScheduleDisabled()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today;
            Properties.Settings.Default.FRIDAY_START = start;
            Properties.Settings.Default.FRIDAY_STOP = end;
            dirtyFlag = true;
        }

        public static DateTime getSaturdayScheduleStart()
        {
            return Properties.Settings.Default.SATURDAY_START;
        }

        public static DateTime getSaturdayScheduleStop()
        {
            return Properties.Settings.Default.SATURDAY_STOP;
        }

        public static bool getSaturdayScheduleAllDayEnabled()
        {
            return isAllDay(Properties.Settings.Default.SATURDAY_START, Properties.Settings.Default.SATURDAY_STOP);
        }

        public static bool getSaturdayScheduleDisabled()
        {
            return isDisabled(Properties.Settings.Default.SATURDAY_START, Properties.Settings.Default.SATURDAY_STOP);
        }

        public static void setSaturdayScheduleStart(DateTime time)
        {
            Properties.Settings.Default.SATURDAY_START = time;
            dirtyFlag = true;
        }

        public static void setSaturdayScheduleStop(DateTime time)
        {
            Properties.Settings.Default.SATURDAY_STOP = time;
            dirtyFlag = true;
        }

        public static void setSaturdayScheduleAllDay()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today.AddDays(1).AddSeconds(-1);
            Properties.Settings.Default.SATURDAY_START = start;
            Properties.Settings.Default.SATURDAY_STOP = end;
            dirtyFlag = true;
        }

        public static void setSaturdayScheduleDisabled()
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today;
            Properties.Settings.Default.SATURDAY_START = start;
            Properties.Settings.Default.SATURDAY_STOP = end;
            dirtyFlag = true;
        }

        public static bool getScheduleAllTheTimeEnabled()
        {
            return getSundayScheduleAllDayEnabled() && getMondayScheduleAllDayEnabled() &&
                    getTuesdayScheduleAllDayEnabled() && getWednesdayScheduleAllDayEnabled() &&
                    getThursdayScheduleAllDayEnabled() && getFridayScheduleAllDayEnabled() &&
                    getSaturdayScheduleAllDayEnabled();
        }

        public static void setScheduleAllTheTime()
        {
            setSundayScheduleAllDay();
            setMondayScheduleAllDay();
            setTuesdayScheduleAllDay();
            setWednesdayScheduleAllDay();
            setThursdayScheduleAllDay();
            setFridayScheduleAllDay();
            setSaturdayScheduleAllDay();
            dirtyFlag = true;
        }

        public static bool getDoNotOverwriteBlockedPicturesEnabled()
        {
            return Properties.Settings.Default.DO_NOT_OVERWRITE_BLOCKED_PICTURES;
        }

        public static void setDoNotOverwriteBlockedPictures(bool enabled)
        {
            Properties.Settings.Default.DO_NOT_OVERWRITE_BLOCKED_PICTURES = enabled;
            dirtyFlag = true;
        }

        public static bool getDontRunOnBatteryEnabled()
        {
            return Properties.Settings.Default.DONT_RUN_ON_BATTERY;
        }

        public static void setDontRunOnBattery(bool enabled)
        {
            Properties.Settings.Default.DONT_RUN_ON_BATTERY = enabled;
            dirtyFlag = true;
        }

        public static string getPanicImage()
        {
            return Environment.ExpandEnvironmentVariables(Properties.Settings.Default.PANIC_IMAGE);
        }

        public static void setPanicImage(string panicImagePath)
        {
            if (panicImagePath == null)
                panicImagePath = "";
            Properties.Settings.Default.PANIC_IMAGE = panicImagePath;
            dirtyFlag = true;
        }

        public static bool getShowCaption()
        {
            return Properties.Settings.Default.SHOW_CAPTION;
        }

        public static void setShowCaption(bool showCaption)
        {
            Properties.Settings.Default.SHOW_CAPTION = showCaption;
            dirtyFlag = true;
        }

        public static bool getShowDomain()
        {
            return Properties.Settings.Default.SHOW_DOMAIN;
        }

        public static void setShowDomain(bool showDomain)
        {
            Properties.Settings.Default.SHOW_DOMAIN = showDomain;
            dirtyFlag = true;
        }

        public static bool getShowTerm()
        {
            return Properties.Settings.Default.SHOW_TERM;
        }

        public static void setShowTerm(bool showTerm)
        {
            Properties.Settings.Default.SHOW_TERM = showTerm;
            dirtyFlag = true;
        }

        public static int getDesktopLabelLocation()
        {
            return Properties.Settings.Default.DESKTOP_LABEL_LOCATION;
        }

        public static void setDesktopLabelLocation(int desktopLabelLocation)
        {
            Properties.Settings.Default.DESKTOP_LABEL_LOCATION = desktopLabelLocation;
            dirtyFlag = true;
        }

        public static List<SettingsSchedule> getSettingsSchedules()
        {
            // Example schedule string:
            // C:\Users\jigglypuff\Desktop\ef1.conf days,false,true,true,true,true,true,false and\nC:\Users\...

            List<SettingsSchedule> schedules = new List<SettingsSchedule>();
            string schedulesString = Properties.Settings.Default.SETTINGS_SCHEDULE;

            string[] scheduleStrings = schedulesString.Split('\n');
            foreach (string scheduleString in scheduleStrings)
            {
                if (scheduleString.Equals(""))
                    continue;
                Console.WriteLine(scheduleString);
                string[] scheduleFields = scheduleString.Split('|');
                string filePath = scheduleFields[0];
                string conditionString = scheduleFields[1];
                string andor = scheduleFields[2];

                SettingsSchedule schedule = new SettingsSchedule();
                schedule.and = andor.Equals("and");
                schedule.file = new SettingsFile(new FileInfo(filePath));

                string[] conditionFields = conditionString.Split(',');
                switch (conditionFields[0])
                {
                    case "times":
                        CertainTimesCondition timesCondition = new CertainTimesCondition();
                        timesCondition.startTime = new TimeSpan(long.Parse(conditionFields[1]));
                        timesCondition.stopTime = new TimeSpan(long.Parse(conditionFields[2]));
                        schedule.condition = timesCondition;
                        break;
                    case "days":
                        CertainDaysCondition daysCondition = new CertainDaysCondition();
                        daysCondition.sunday = bool.Parse(conditionFields[1]);
                        daysCondition.monday = bool.Parse(conditionFields[2]);
                        daysCondition.tuesday = bool.Parse(conditionFields[3]);
                        daysCondition.wednesday = bool.Parse(conditionFields[4]);
                        daysCondition.thursday = bool.Parse(conditionFields[5]);
                        daysCondition.friday = bool.Parse(conditionFields[6]);
                        daysCondition.saturday = bool.Parse(conditionFields[7]);
                        schedule.condition = daysCondition;
                        break;
                    case "months":
                        CertainMonthsCondition monthsCondition = new CertainMonthsCondition();
                        monthsCondition.january = bool.Parse(conditionFields[1]);
                        monthsCondition.february = bool.Parse(conditionFields[2]);
                        monthsCondition.march = bool.Parse(conditionFields[3]);
                        monthsCondition.april = bool.Parse(conditionFields[4]);
                        monthsCondition.may = bool.Parse(conditionFields[5]);
                        monthsCondition.june = bool.Parse(conditionFields[6]);
                        monthsCondition.july = bool.Parse(conditionFields[7]);
                        monthsCondition.august = bool.Parse(conditionFields[8]);
                        monthsCondition.september = bool.Parse(conditionFields[9]);
                        monthsCondition.october = bool.Parse(conditionFields[10]);
                        monthsCondition.november = bool.Parse(conditionFields[11]);
                        monthsCondition.december = bool.Parse(conditionFields[12]);
                        schedule.condition = monthsCondition;
                        break;
                }

                schedules.Add(schedule);
            }

            return schedules;
        }

        public static void setSettingsSchedules(List<SettingsSchedule> schedules)
        {
            // Put the schedules into string form and into the Settings
            string scheduleString = "";
            foreach (SettingsSchedule schedule in schedules)
            {
                scheduleString += schedule.file.info.FullName + "|";
                scheduleString += schedule.condition + "|";
                scheduleString += schedule.and ? "and" : "or";
                scheduleString += "\n";
            }
            Properties.Settings.Default.SETTINGS_SCHEDULE = scheduleString;

            dirtyFlag = true;
        }

        public static Hashtable getPropertiesFromSettingsFile(string settingsFileName)
        {
            Hashtable properties = new Hashtable();

            try
            {
                using (StreamReader reader = new StreamReader(settingsFileName))
                {
                    while (reader.Peek() != -1)
                    {
                        string line = reader.ReadLine();
                        string[] lineParts = line.Split(':');
                        string propertyName = lineParts[0];
                        string propertyValue = lineParts.Length > 1 ? lineParts[1] : "";
                        if (propertyValue.Contains(','))
                        {
                            string[] values = propertyValue.Split(',');
                            propertyValue = "";
                            foreach (string value in values)
                            {
                                propertyValue += Base64Util.Base64Decode(value) + ",";
                            }
                            if (propertyValue.EndsWith(","))
                                propertyValue = propertyValue.Substring(0, propertyValue.Length - 1);
                        }
                        else
                            propertyValue = Base64Util.Base64Decode(propertyValue);
                        properties.Add(propertyName, propertyValue);
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return properties;
        }

        public static bool isScheduledNow()
        {
            bool inSchedule = true;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    inSchedule &= getSundayScheduleStart().TimeOfDay <= DateTime.Now.TimeOfDay;
                    inSchedule &= DateTime.Now.TimeOfDay <= getSundayScheduleStop().TimeOfDay;
                    break;
                case DayOfWeek.Monday:
                    inSchedule &= getMondayScheduleStart().TimeOfDay <= DateTime.Now.TimeOfDay;
                    inSchedule &= DateTime.Now.TimeOfDay <= getMondayScheduleStop().TimeOfDay;
                    break;
                case DayOfWeek.Tuesday:
                    inSchedule &= getTuesdayScheduleStart().TimeOfDay <= DateTime.Now.TimeOfDay;
                    inSchedule &= DateTime.Now.TimeOfDay <= getTuesdayScheduleStop().TimeOfDay;
                    break;
                case DayOfWeek.Wednesday:
                    inSchedule &= getWednesdayScheduleStart().TimeOfDay <= DateTime.Now.TimeOfDay;
                    inSchedule &= DateTime.Now.TimeOfDay <= getWednesdayScheduleStop().TimeOfDay;
                    break;
                case DayOfWeek.Thursday:
                    inSchedule &= getThursdayScheduleStart().TimeOfDay <= DateTime.Now.TimeOfDay;
                    inSchedule &= DateTime.Now.TimeOfDay <= getThursdayScheduleStop().TimeOfDay;
                    break;
                case DayOfWeek.Friday:
                    inSchedule &= getFridayScheduleStart().TimeOfDay <= DateTime.Now.TimeOfDay;
                    inSchedule &= DateTime.Now.TimeOfDay <= getFridayScheduleStop().TimeOfDay;
                    break;
                case DayOfWeek.Saturday:
                    inSchedule &= getSaturdayScheduleStart().TimeOfDay <= DateTime.Now.TimeOfDay;
                    inSchedule &= DateTime.Now.TimeOfDay <= getSaturdayScheduleStop().TimeOfDay;
                    break;
            }

            return inSchedule;
        }
    }
}
