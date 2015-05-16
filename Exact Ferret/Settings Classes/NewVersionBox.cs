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
    public partial class NewVersionBox : Form
    {
        public NewVersionBox()
        {
            InitializeComponent();

            welcomeLabel.Text += " " + Application.ProductVersion;
        }

        private void helpLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string helpFileLocation = PropertiesManager.HELP_FILE_PATH;
            Process.Start(helpFileLocation);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
