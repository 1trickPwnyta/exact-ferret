using Http;
using Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret
{
    class UpdateManager
    {
        private const int DISCRETE_SOFTWARE_VERSION = 18;

        private const string VERSION_QUERY_URL = "https://exactferret.kangaroostandard.com/version";
        private const string UPDATE_URL = "https://exactferret.kangaroostandard.com/update";
        private const string UPDATE_TEMP_FILE_NAME = "UpdateExactFerret.exe";
        private const string UPDATE_TRUSTED_CERT_ISSUER = "CN=kangaroostandard.com";
        private const string UPDATE_TRUSTED_CERT_THUMBPRINT = "95c2569335dcdd4980a4bc0a5c3cab3b3c39b159";
        private const string UPDATE_TRUSTED_CERT_THUMBPRINT_OLD = "64f22c453dec5820fe027738cc38ab75d93491c2";

        public static void checkForUpdates()
        {
            Log.trace("Checking for software updates.");
            Log.trace("Will trust only the server certificate with issuer " + UPDATE_TRUSTED_CERT_ISSUER + 
                    " and thumbprint " + UPDATE_TRUSTED_CERT_THUMBPRINT + " or " + UPDATE_TRUSTED_CERT_THUMBPRINT_OLD);
            HttpUtil.trustCertificates(UPDATE_TRUSTED_CERT_ISSUER, UPDATE_TRUSTED_CERT_THUMBPRINT, UPDATE_TRUSTED_CERT_THUMBPRINT_OLD);

            int latestVersion;
            try
            {
                latestVersion = int.Parse(HttpUtil.get("https://exactferret.kangaroostandard.com/version"));
                Log.trace("Latest version: " + latestVersion);
            }
            catch (Exception e)
            {
                Log.warning("Failed to check the latest software version at " + VERSION_QUERY_URL);
                return;
            }

            if (latestVersion > DISCRETE_SOFTWARE_VERSION)
            {
                Log.info("Downloading a new software version from " + UPDATE_URL);

                string tempDir = Environment.ExpandEnvironmentVariables("%TEMP%");
                string installerFile = tempDir + "\\" + UPDATE_TEMP_FILE_NAME;

                try
                {
                    if (!HttpUtil.downloadFile(UPDATE_URL, installerFile))
                        throw new Exception();
                }
                catch (Exception e)
                {
                    Log.warning("Failed to download the new version of Exact Ferret from " + UPDATE_URL);
                    return;
                }

                Log.trace("Launching the installer for the new version.");

                Process process = new Process();
                process.StartInfo.FileName = installerFile;
                process.StartInfo.Arguments = "-silent";
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
