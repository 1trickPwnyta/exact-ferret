using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret
{
    public partial class TrayFormHidden : Form
    {
        public TrayFormHidden()
        {
            InitializeComponent();
            
            // Start the ping thread
            Thread thread = new Thread(new ThreadStart(() => pingLoop(pingResponse)));
            thread.Start();
        }

        private void pingLoop(Action<bool> consumerFunction)
        {
            while (true)
            {
                consumerFunction(Communication.ping(0));
                Thread.Sleep(Communication.PING_INTERVAL_MILLISECONDS);
            }
        }

        public void pingResponse(bool success)
        {
            if (success)
            {
                SetControlPropertyValue(contextMenuStrip1, "Start", "Enabled", false);
                SetControlPropertyValue(contextMenuStrip1, "Stop", "Enabled", true);
            }
            else
            {
                SetControlPropertyValue(contextMenuStrip1, "Start", "Enabled", true);
                SetControlPropertyValue(contextMenuStrip1, "Stop", "Enabled", false);
            }
        }

        delegate void SetControlValueCallback(Control oControl, string itemName, string propName, object propValue);
        private static void SetControlPropertyValue(Control oControl, string itemName, string propName, object propValue)
        {
            if (oControl.InvokeRequired)
            {
                SetControlValueCallback d = new SetControlValueCallback(SetControlPropertyValue);
                oControl.Invoke(d, new object[] { oControl, itemName, propName, propValue });
            }
            else
            {
                ToolStrip toolStrip = (ToolStrip)oControl;
                foreach (ToolStripMenuItem item in toolStrip.Items)
                {
                    if (item.Text.ToUpper() == itemName.ToUpper())
                    {
                        Type t = typeof(ToolStripMenuItem);
                        PropertyInfo[] props = t.GetProperties();
                        foreach (PropertyInfo p in props)
                        {
                            if (p.Name.ToUpper() == propName.ToUpper())
                            {
                                p.SetValue(item, propValue, null);
                            }
                        }
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Communication.stopExactFerret(0);
            notifyIcon1.Dispose();
            Environment.Exit(0);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string settingsPath = Environment.ExpandEnvironmentVariables("%EXACTFERRET%\\Exact Ferret.exe");
            Process p = new Process();
            p.StartInfo.FileName = settingsPath;
            p.StartInfo.Arguments = "-settings";
            p.Start();
        }

        private void quickRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.ExpandEnvironmentVariables("%EXACTFERRET%\\Exact Ferret.exe");
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = "-quick";
            p.Start();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                settingsToolStripMenuItem_Click(sender, e);
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Communication.startExactFerret(0);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Communication.stopExactFerret(0);
        }

        private void panicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.ExpandEnvironmentVariables("%EXACTFERRET%\\Exact Ferret.exe");
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = "-panic";
            p.Start();
        }
    }
}
