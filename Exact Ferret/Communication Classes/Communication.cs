using Exact_Ferret.Settings_Classes;
using Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret
{
    public class Communication
    {
        public const int COMMUNICATION_PORT_BASE = 29090;
        public const int PING_INTERVAL_MILLISECONDS = 1000;

        private static DateTime lastSleepTime;
        private static int sleepSeconds = 0;
        private static int id;

        public static void startListening(int id)
        {
            // ID = 0: Regular background process
            // ID = 1: Desktop label process
            Communication.id = id;

            // Open communication socket on a new thread
            Thread thread = new Thread(new ThreadStart(() => { Communication.openCommunicationSocket(COMMUNICATION_PORT_BASE + id); }));
            thread.Start();
        }

        private static void openCommunicationSocket(int port)
        {
            Log.trace("Opening communication socket on port " + port);

            try
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:" + port + "/");
                try
                {
                    listener.Start();
                }
                catch (Exception e)
                {
                    Log.error("Could not open the communication socket on port " + port + ". The port may already be in use.");
                    if (id == 0)
                        MessageBox.Show("Exact Ferret could not start. See the log for details.", "Exact Ferret", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }

                bool stopping = false;
                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
                    string responseString = "";

                    string requestCommand = request.QueryString.Get("command");
                    if (requestCommand == null)
                        requestCommand = "";

                    switch (requestCommand.ToLower())
                    {
                        case "ping":
                            responseString = "{\"success\":\"true\"}";
                            break;
                        case "countdown":
                            int countdown = sleepSeconds - (int)(DateTime.Now - lastSleepTime).TotalSeconds;
                            responseString = "{\"success\":\"true\",\"countdown\":\"" + countdown + "\"}";
                            break;
                        case "stop":
                            Log.trace("Received 'stop' command.");
                            stopping = true;
                            responseString = "{\"success\":\"true\"}";
                            break;
                        default:
                            Log.warning("Received unknown command: '" + requestCommand + "'.");
                            responseString = "{\"success\":\"false\",\"reason\":\"unknown command\"}";
                            response.StatusCode = 400;
                            break;
                    }

                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;

                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();

                    if (stopping)
                    {
                        if (id == 0)
                            Log.info("The Exact Ferret background process is stopping.");
                        listener.Stop();
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception e)
            {
                Log.error("An unhandled exception occurred in the communication thread: " + e.Message + ": " + e.StackTrace);
                Log.trace("The communication thread is stopping.");
            }
        }

        public static bool stopExactFerret(int id)
        {
            WebRequest request = WebRequest.Create("http://localhost:" + (Communication.COMMUNICATION_PORT_BASE + id) + "/?command=stop");

            bool success;
            try
            {
                success = ((CommunicationResponse)new DataContractJsonSerializer(typeof(CommunicationResponse)).ReadObject(request.GetResponse().GetResponseStream())).success;
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public static void startExactFerret(int id)
        {
            // If no system tray running, start it instead (it will start Exact Ferret)
            Process[] pname = Process.GetProcessesByName("Exact Ferret Tray Icon");
            if (pname.Length == 0)
            {
                string executableLocation = PropertiesManager.getInstallationDirectoryPath() + 
                        "\\" + PropertiesManager.TRAY_ICON_EXECUTABLE_NAME;
                Process process = new Process();
                process.StartInfo.FileName = executableLocation;
                process.Start();
            }
            else
            {
                string executableLocation = PropertiesManager.getInstallationDirectoryPath() +
                        "\\" + PropertiesManager.EXACT_FERRET_EXECUTABLE_NAME;
                Process process = new Process();
                process.StartInfo.FileName = executableLocation;
                if (id == 1)
                    process.StartInfo.Arguments = "-k";
                process.Start();
            }
        }

        public static void startExactFerretWithDelay()
        {
            string executableLocation = PropertiesManager.getInstallationDirectoryPath() +
                    "\\" + PropertiesManager.EXACT_FERRET_EXECUTABLE_NAME;
            Process process = new Process();
            process.StartInfo.FileName = executableLocation;
            process.StartInfo.Arguments = "-delay";
            process.Start();
        }

        public static bool ping(int id)
        {
            WebRequest request = WebRequest.Create("http://localhost:" + (Communication.COMMUNICATION_PORT_BASE + id) + "/?command=ping");
            request.Timeout = PING_INTERVAL_MILLISECONDS;

            bool success;
            try
            {
                success = ((CommunicationResponse)new DataContractJsonSerializer(typeof(CommunicationResponse)).ReadObject(request.GetResponse().GetResponseStream())).success;
            }
            catch
            {
                success = false;
            }

            return success;
        }

        public static int getCountdown()
        {
            WebRequest request = WebRequest.Create("http://localhost:" + Communication.COMMUNICATION_PORT_BASE + "/?command=countdown");
            request.Timeout = PING_INTERVAL_MILLISECONDS;

            int seconds;
            try
            {
                seconds = ((CommunicationResponse)new DataContractJsonSerializer(typeof(CommunicationResponse)).ReadObject(request.GetResponse().GetResponseStream())).countdown;
            }
            catch (Exception e1)
            {
                seconds = -1;
            }

            return seconds;
        }

        public static void setRunCountdown(int sleepSeconds)
        {
            lastSleepTime = DateTime.Now;
            Communication.sleepSeconds = sleepSeconds;
        }
    }
}
