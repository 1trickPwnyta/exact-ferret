using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Logging
{
    class Log
    {
        public const int NONE = 0;
	    public const int ERROR = 1;
	    public const int WARNING = 2;
        public const int INFO = 3;
	    public const int TRACE = 4;
        public const int SPECIAL = 5;

        private static int logLevel = 1;
        private static string logFileLocation = null;
        private static int rolloverKBytes = 10;

        private static bool showedFailureMessage = false;

        public static void clearOldLogs(int minimumDaysOld)
        {
            if (logFileLocation == null)
            {
                throw new Exception("An attempt to clear old logs was made before setting the log file location.");
            }

            FileInfo logFile = new FileInfo(logFileLocation);
            DirectoryInfo logFolder = Directory.GetParent(logFile.FullName);

            if (logFolder.Exists)
            {
                DateTime now = DateTime.Now;
                foreach (FileInfo file in logFolder.GetFiles())
                {
                    if (file.LastWriteTime.AddDays(minimumDaysOld) < now)
                    {
                        file.Delete();
                    }
                }
            }
        }

        public static void setLogLevel(int logLevel)
        {
            Log.logLevel = logLevel;
        }

        public static int getLogLevel()
        {
            return logLevel;
        }

        public static void setLogFileLocation(string path)
        {
            logFileLocation = path;
        }

        public static string getLogFileLocation()
        {
            return logFileLocation;
        }

        public static void setLogRolloverKBytes(int kBytes)
        {
            rolloverKBytes = kBytes;
        }

        public static int getLogRolloverKBytes()
        {
            return rolloverKBytes;
        }

        public static void error(string message)
        {
            if (logLevel >= ERROR)
                write("  ERROR: " + message);
        }

        public static void warning(string message)
        {
            if (logLevel >= WARNING)
                write("WARNING: " + message);
        }

        public static void info(string message)
        {
            if (logLevel >= INFO)
                write("   INFO: " + message);
        }

        public static void trace(string message)
        {
            if (logLevel >= TRACE)
                write("  TRACE: " + message);
        }

        public static void special(string message, string tag)
        {
            if (logLevel >= SPECIAL)
                write(tag + ": " + message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void write(string message)
        {
            using (StreamWriter writer = openLog())
            {
                if (writer != null)
                    writer.WriteLine(DateTime.Now + " " + message);
            }
        }

        private static StreamWriter openLog()
        {
            if (logFileLocation == null)
            {
                throw new Exception("An attempt to write to the log was made before setting the log file location.");
            }

            FileInfo logFile = new FileInfo(logFileLocation);
            DirectoryInfo logFolder = Directory.GetParent(logFile.FullName);

            try {
                if (!logFolder.Exists)
                    logFolder.Create();

                if (logFile.Exists && logFile.Length >= rolloverKBytes * 1024)
                {
                    logFile.MoveTo(logFile.FullName + "." + DateTime.Now.ToFileTime());
                    logFile = new FileInfo(logFileLocation);
                }

                return new StreamWriter(logFile.FullName, true);
            }
            catch (Exception e)
            {
                if (!showedFailureMessage)
                {
                    MessageBox.Show(null, "Could not write to log file at " + logFile.FullName, "Exact Ferret", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    showedFailureMessage = true;
                }
                return null;
            }
        }
    }
}
