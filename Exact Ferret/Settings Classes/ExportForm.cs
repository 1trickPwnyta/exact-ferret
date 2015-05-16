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
    public partial class ExportForm : Form
    {
        public ExportForm()
        {
            InitializeComponent();
        }

        public string[] getPropertyNames()
        {
            if (allCheckBox.Checked)
                return null;

            List<string> propertyNames = new List<string>();

            if (generalCheckBox.Checked)
            {
                propertyNames.Add("startAtLogon");
                propertyNames.Add("AUTO_UPDATE");
            }
            if (behaviorCheckBox.Checked)
            {
                propertyNames.Add("IMAGE_LOCATION");
                propertyNames.Add("MAX_OLD_IMAGES");
                propertyNames.Add("HTTP_REQUEST_TIMEOUT");
                propertyNames.Add("DOWNLOAD_ATTEMPTS");
                propertyNames.Add("CHANGE_LOCK_SCREEN");
                propertyNames.Add("CHANGE_WALLPAPER");
                propertyNames.Add("WALLPAPER_STYLE");
            }
            if (searchCheckBox.Checked)
            {
                propertyNames.Add("DICTIONARY_FILE");
                propertyNames.Add("SEARCH_TERM_MODIFIER");
                propertyNames.Add("MINIMUM_IMAGE_HEIGHT");
                propertyNames.Add("MAXIMUM_IMAGE_HEIGHT");
                propertyNames.Add("MAXIMUM_IMAGE_WIDTH");
                propertyNames.Add("MINIMUM_IMAGE_WIDTH");
                propertyNames.Add("MAXIMUM_IMAGE_RATIO");
                propertyNames.Add("MINIMUM_IMAGE_RATIO");
                propertyNames.Add("COLORS_IN_IMAGE");
                propertyNames.Add("TYPE_OF_IMAGE");
                propertyNames.Add("SAFE_SEARCH");
                propertyNames.Add("SEARCH_DOMAIN");
            }
            if (processCheckBox.Checked)
            {
                propertyNames.Add("INTERVAL_MINUTES");
                propertyNames.Add("INTERVAL_SECONDS");
                propertyNames.Add("FRIDAY_START");
                propertyNames.Add("SATURDAY_START");
                propertyNames.Add("WEDNESDAY_STOP");
                propertyNames.Add("FRIDAY_STOP");
                propertyNames.Add("MONDAY_START");
                propertyNames.Add("THURSDAY_START");
                propertyNames.Add("SUNDAY_STOP");
                propertyNames.Add("TUESDAY_START");
                propertyNames.Add("TUESDAY_STOP");
                propertyNames.Add("SATURDAY_STOP");
                propertyNames.Add("WEDNESDAY_START");
                propertyNames.Add("MONDAY_STOP");
                propertyNames.Add("THURSDAY_STOP");
                propertyNames.Add("SUNDAY_START");
                propertyNames.Add("DONT_RUN_ON_BATTERY");
            }
            if (desktopLabelCheckBox.Checked)
            {
                propertyNames.Add("enableDesktopLabel");
                propertyNames.Add("SHOW_CAPTION");
                propertyNames.Add("SHOW_DOMAIN");
                propertyNames.Add("SHOW_TERM");
                propertyNames.Add("DESKTOP_LABEL_LOCATION");
            }
            if (panicModeCheckBox.Checked)
            {
                propertyNames.Add("PANIC_IMAGE");
            }
            if (apiCheckBox.Checked)
            {
                propertyNames.Add("CYCLE_AT_API_LIMIT");
                propertyNames.Add("API_LIMIT_WARNING");
                propertyNames.Add("GOOGLE_API_KEY");
                propertyNames.Add("SINGLE_API_LIMIT_WARNING");
                propertyNames.Add("SEARCH_ENGINE_ORDER");
                propertyNames.Add("BING_ACCESS_TOKEN");
            }
            if (blockedCheckBox.Checked)
            {
                propertyNames.Add("BLOCKED_IMAGES");
                propertyNames.Add("BLOCKED_DOMAINS");
            }
            if (shortcutCheckBox.Checked)
            {
                propertyNames.Add("QUICK_RUN_SHORTCUT");
                propertyNames.Add("PANIC_SHORTCUT");
                propertyNames.Add("SETTINGS_SHORTCUT");
            }
            if (logCheckBox.Checked)
            {
                propertyNames.Add("LOG_LEVEL");
                propertyNames.Add("LOG_RETENTION_DAYS");
                propertyNames.Add("LOG_FILE_LOCATION");
                propertyNames.Add("LOG_ROLLOVER_KBYTES");
            }

            return propertyNames.ToArray();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void allCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settingsBox.Enabled = !allCheckBox.Checked;
        }
    }
}
