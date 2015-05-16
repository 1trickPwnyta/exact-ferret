using Exact_Ferret;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Tray_Icon
{
    class TrayContext : ApplicationContext
    {
        private IContainer components = null;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem startToolStripMenuItem;
        private ToolStripMenuItem stopToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem quickRunToolStripMenuItem;
        private ToolStripMenuItem panicToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;

        public TrayContext()
        {
            this.components = new Container();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickRunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrayFormHidden));

            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Exact Ferret";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.panicToolStripMenuItem,
            this.toolStripSeparator1,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.quickRunToolStripMenuItem,
            this.toolStripSeparator2,
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(130, 114);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Enabled = false;
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Enabled = false;
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // quickRunToolStripMenuItem
            // 
            this.quickRunToolStripMenuItem.Name = "quickRunToolStripMenuItem";
            this.quickRunToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.quickRunToolStripMenuItem.Text = "Quick Run";
            this.quickRunToolStripMenuItem.Click += new System.EventHandler(this.quickRunToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // panicToolStripMenuItem
            // 
            this.panicToolStripMenuItem.Name = "panicToolStripMenuItem";
            this.panicToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.panicToolStripMenuItem.Text = "Panic";
            this.panicToolStripMenuItem.Click += new System.EventHandler(this.panicToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            
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
                //SetControlPropertyValue(contextMenuStrip1, "Start", "Enabled", false);
                //SetControlPropertyValue(contextMenuStrip1, "Stop", "Enabled", true);
                startToolStripMenuItem.Enabled = false;
                stopToolStripMenuItem.Enabled = true;
            }
            else
            {
                //SetControlPropertyValue(contextMenuStrip1, "Start", "Enabled", true);
                //SetControlPropertyValue(contextMenuStrip1, "Stop", "Enabled", false);
                startToolStripMenuItem.Enabled = true;
                stopToolStripMenuItem.Enabled = false;
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

        private void panicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.ExpandEnvironmentVariables("%EXACTFERRET%\\Exact Ferret.exe");
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = "-panic";
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
    }
}
