using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret.Settings_Classes
{
    public partial class StringInput : Form
    {
        public string UserInput { get { return answerTextBox.Text; } }

        public StringInput(string question, string title, string defaultValue)
        {
            InitializeComponent();

            questionLabel.Text = question;
            Text = title;
            answerTextBox.Text = defaultValue;
            answerTextBox.SelectAll();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
