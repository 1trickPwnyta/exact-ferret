using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret.Settings_Classes
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string helpFileLocation = PropertiesManager.HELP_FILE_PATH;
            Process.Start(helpFileLocation);
        }

        private void apiKeyBox_TextChanged(object sender, EventArgs e)
        {
            if (googleApiKeyBox.Text.Equals("") && bingAccessTokenBox.Text.Equals(""))
                finishButton.Enabled = false;
            else
                finishButton.Enabled = true;
        }
    }
}
