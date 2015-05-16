using Exact_Ferret.Settings_Classes;
using Exact_Ferret.Utility_Classes;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret
{
    public partial class SettingsForm : Form
    {
        private const int IMAGE_PREVIEW_UPDATE_INTERVAL_MILLISECONDS = 1000;
        private const int SCHEDULE_FORM_UPDATE_INTERVAL_MILLISECONDS = 15000;

        private bool visitedGeneralTab = false;
        private bool visitedBehaviorTab = false;
        private bool visitedProcessTab = false;
        private bool visitedApiTab = false;
        private bool visitedSearchTab = false;
        private bool visitedPanicTab = false;
        private bool visitedShortcutsTab = false;
        private bool lockPictureButtons = false;
        private bool freezeCustomSettingControls = false;
        private object imagePreviewKey = new object();
        private Hashtable savedTooltips = new Hashtable();

        private int threadsRunning = 0;
        private bool exiting = false;

        private bool processEvents = false;
        private int rememberedTabIndex = -1;

        private string lastTermUsedInSearch = "";

        public SettingsForm()
        {
            InitializeComponent();

            // Set the state of the start at logon checkbox before adding the checkedchanged event
            startOnLogOnCheckBox.Checked = PropertiesManager.getStartAtLogonEnabled();
            startOnLogOnCheckBox.CheckedChanged += new System.EventHandler(startOnLogOnCheckBox_CheckedChanged);

            // Set the state of the enable desktop label checkbox before adding the checkedchanged event
            enableDesktopLabelCheckBox.Checked = PropertiesManager.getEnableDesktopLabel();
            enableDesktopLabelCheckBox.CheckedChanged += new System.EventHandler(enableDesktopLabelCheckBox_CheckedChanged);

            // Set the state of the shortcut dropdowns before adding the value changed event
            // Now that they are bound to settings, don't need to set their state
            //quickRunShortcutComboBox.SelectedIndex = PropertiesManager.getQuickRunShortcut();
            quickRunShortcutComboBox.SelectionChangeCommitted += new System.EventHandler(quickRunShortcutComboBox_SelectionChangeCommitted);
            //panicShortcutComboBox.SelectedIndex = PropertiesManager.getPanicShortcut();
            panicShortcutComboBox.SelectionChangeCommitted += new System.EventHandler(panicShortcutComboBox_SelectionChangeCommitted);
            //settingsShortcutComboBox.SelectedIndex = PropertiesManager.getSettingsShortcut();
            settingsShortcutComboBox.SelectionChangeCommitted += new System.EventHandler(settingsShortcutComboBox_SelectionChangeCommitted);

            // Set the state of the schedule radio button
            allTheTimeRadioButton.Checked = PropertiesManager.getScheduleAllTheTimeEnabled();
            scheduleRadioButton.Checked = !allTheTimeRadioButton.Checked;
            scheduleBox.Enabled = !allTheTimeRadioButton.Checked;
            allTheTimeRadioButton.CheckedChanged += new System.EventHandler(allTheTimeRadioButton_CheckedChanged);
            scheduleRadioButton.CheckedChanged += new System.EventHandler(scheduleRadioButton_CheckedChanged);

            // Set the state of the panic mode radio buttons
            panicRemoveRadioButton.Checked = PropertiesManager.getPanicImage().Equals("");
            panicSwitchRadioButton.Checked = !panicRemoveRadioButton.Checked;
            panicSwitchTextBox.Enabled = !panicRemoveRadioButton.Checked;
            panicImageBrowseButton.Enabled = !panicRemoveRadioButton.Checked;
            panicRemoveRadioButton.CheckedChanged += new System.EventHandler(panicRemoveRadioButton_CheckedChanged);
            panicSwitchRadioButton.CheckedChanged += new System.EventHandler(panicSwitchRadioButton_CheckedChanged);

            // Set the state of the daily schedule check boxes and time pickers
            sundayScheduleCheckBox.Checked = !PropertiesManager.getSundayScheduleDisabled();
            sundayScheduleStartPicker.Enabled = sundayScheduleCheckBox.Checked;
            sundayScheduleStopPicker.Enabled = sundayScheduleCheckBox.Checked;
            mondayScheduleCheckBox.Checked = !PropertiesManager.getMondayScheduleDisabled();
            mondayScheduleStartPicker.Enabled = mondayScheduleCheckBox.Checked;
            mondayScheduleStopPicker.Enabled = mondayScheduleCheckBox.Checked;
            tuesdayScheduleCheckBox.Checked = !PropertiesManager.getTuesdayScheduleDisabled();
            tuesdayScheduleStartPicker.Enabled = tuesdayScheduleCheckBox.Checked;
            tuesdayScheduleStopPicker.Enabled = tuesdayScheduleCheckBox.Checked;
            wednesdayScheduleCheckBox.Checked = !PropertiesManager.getWednesdayScheduleDisabled();
            wednesdayScheduleStartPicker.Enabled = wednesdayScheduleCheckBox.Checked;
            wednesdayScheduleStopPicker.Enabled = wednesdayScheduleCheckBox.Checked;
            thursdayScheduleCheckBox.Checked = !PropertiesManager.getThursdayScheduleDisabled();
            thursdayScheduleStartPicker.Enabled = thursdayScheduleCheckBox.Checked;
            thursdayScheduleStopPicker.Enabled = thursdayScheduleCheckBox.Checked;
            fridayScheduleCheckBox.Checked = !PropertiesManager.getFridayScheduleDisabled();
            fridayScheduleStartPicker.Enabled = fridayScheduleCheckBox.Checked;
            fridayScheduleStopPicker.Enabled = fridayScheduleCheckBox.Checked;
            saturdayScheduleCheckBox.Checked = !PropertiesManager.getSaturdayScheduleDisabled();
            saturdayScheduleStartPicker.Enabled = saturdayScheduleCheckBox.Checked;
            saturdayScheduleStopPicker.Enabled = saturdayScheduleCheckBox.Checked;
            sundayScheduleCheckBox.CheckedChanged += new System.EventHandler(sundayScheduleCheckBox_CheckedChanged);
            mondayScheduleCheckBox.CheckedChanged += new System.EventHandler(mondayScheduleCheckBox_CheckedChanged);
            tuesdayScheduleCheckBox.CheckedChanged += new System.EventHandler(tuesdayScheduleCheckBox_CheckedChanged);
            wednesdayScheduleCheckBox.CheckedChanged += new System.EventHandler(wednesdayScheduleCheckBox_CheckedChanged);
            thursdayScheduleCheckBox.CheckedChanged += new System.EventHandler(thursdayScheduleCheckBox_CheckedChanged);
            fridayScheduleCheckBox.CheckedChanged += new System.EventHandler(fridayScheduleCheckBox_CheckedChanged);
            saturdayScheduleCheckBox.CheckedChanged += new System.EventHandler(saturdayScheduleCheckBox_CheckedChanged);

            // Set the state of the wallpaper style combo box
            wallpaperStyleComboBox.Enabled = changeWallpaperCheckBox.Checked;

            // Set the state of the desktop label group box
            desktopLabelGroupBox.Enabled = enableDesktopLabelCheckBox.Checked;

            // Bind the blocked image lists to properties

            BindingSource blockedImageSource = new BindingSource();
            blockedImageSource.DataSource = PropertiesManager.getBlockedImagesDataSource();
            blockedImagesList.DataSource = blockedImageSource;

            BindingSource blockedDomainSource = new BindingSource();
            blockedDomainSource.DataSource = PropertiesManager.getBlockedDomainsDataSource();
            blockedDomainsList.DataSource = blockedDomainSource;

            BindingSource searchEngineOrderSource = new BindingSource();
            searchEngineOrderSource.DataSource = PropertiesManager.getSearchEngineOrderDataSource();
            searchEngineOrderListBox.DataSource = searchEngineOrderSource;


            // Start the preview update thread
            Thread previewUpdateThread = new Thread(new ThreadStart(updateImagePreviewLoop));
            threadsRunning++;
            previewUpdateThread.Start();

            // Start the ping thread
            Thread thread = new Thread(new ThreadStart(() => pingLoop(pingResponse)));
            threadsRunning++;
            thread.Start();

            // Start the settings schedule thread
            Thread settingsScheduleThread = new Thread(new ThreadStart(updateFormFromScheduleLoop));
            threadsRunning++;
            settingsScheduleThread.Start();

            // Check if this is a newly installed version
            string[] productVersionNumbers = Application.ProductVersion.Split('.');
            if (!PropertiesManager.getSoftwareMinorVersion().Equals(productVersionNumbers[0] + "." + productVersionNumbers[1]))
            {
                PropertiesManager.setSoftwareVersion(Application.ProductVersion);
                NewVersionBox newVersionBox = new NewVersionBox();
                newVersionBox.ShowDialog();
            }

            // Check for the API key
            if (PropertiesManager.getGoogleApiKey().Equals("") && PropertiesManager.getBingAccessToken().Equals(""))
            {
                Welcome welcome = new Welcome();
                DialogResult result = welcome.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string googleApiKey = welcome.googleApiKeyBox.Text;
                    string bingAccessToken = welcome.bingAccessTokenBox.Text;
                    PropertiesManager.setGoogleApiKey(googleApiKey);
                    PropertiesManager.setBingAccessToken(bingAccessToken);
                    PropertiesManager.save();
                    
                    // Restart the background process
                    if (Communication.ping(0)){
                        Communication.stopExactFerret(0);
                        Communication.startExactFerret(0);
                    }

                    // Restart the desktop label
                    if (Communication.ping(1))
                        Communication.stopExactFerret(1);
                    if (PropertiesManager.getEnableDesktopLabel())
                        Communication.startExactFerret(1);
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void exitWithoutSaving(object sender, EventArgs e)
        {
            // Exit without saving changes
            PropertiesManager.dirtyFlag = false;

            // Wait for all threads to die
            exiting = true;
            while (threadsRunning > 0)
            {
                Thread.Sleep(10);
            }

            Environment.Exit(0);
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (PropertiesManager.dirtyFlag)
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save your changes before exiting?", 
                        "Exact Ferret", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        if (!saveAllSettings(true))
                            e.Cancel = true;
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }

            // Wait for all threads to die
            exiting = true;
            while (threadsRunning > 0)
            {
                Thread.Sleep(10);
            }
        }

        private void saveAndExit(object sender, EventArgs e)
        {
            // Save changes and exit
            if (saveAllSettings(true))
            {
                // Wait for all threads to die
                exiting = true;
                while (threadsRunning > 0)
                {
                    Thread.Sleep(10);
                }

                Environment.Exit(0);
            }
        }

        private void applyChanges(object sender, EventArgs e)
        {
            saveAllSettings(true);
        }

        private void SetDirtyBit(object sender, EventArgs e)
        {
            if (processEvents)
                PropertiesManager.dirtyFlag = true;
        }

        private void NumericUpDown_KeyDown_SetDirtyBit(object sender, KeyEventArgs e)
        {
            if (processEvents)
                PropertiesManager.dirtyFlag = true;
        }

        private void logFileBrowseButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog Dialog = new SaveFileDialog();
            Dialog.DefaultExt = "log";
            Dialog.Filter = "Log files|*.log";
            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                logFileLocationTextBox.Text = Dialog.FileName;
        }

        private void imageLocationBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                imageLocationTextBox.Text = Dialog.SelectedPath;
        }

        private bool saveAllSettings(bool restartNoDelay)
        {
            if (PropertiesManager.dirtyFlag)
            {
                // Validation warning
                List<string> warnings = new List<string>();

                // Validate shortcuts
                int quickRunShortcut = quickRunShortcutComboBox.SelectedIndex;
                int panicShortcut = panicShortcutComboBox.SelectedIndex;
                int settingsShortcut = settingsShortcutComboBox.SelectedIndex;
                if (visitedShortcutsTab && ((quickRunShortcut != 0 && (quickRunShortcut == panicShortcut || quickRunShortcut == settingsShortcut)) || (panicShortcut != 0 && panicShortcut == settingsShortcut)))
                {
                    warnings.Add("You have selected the same shortcut key for more than one action.");
                }

                // Validate panic picture
                string panicPicture = Environment.ExpandEnvironmentVariables(panicSwitchTextBox.Text);
                if (visitedPanicTab && !panicPicture.Equals("") && !panicPicture.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) && !panicPicture.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    warnings.Add("Only JPG images can be used as the panic picture.");
                }
                if (visitedPanicTab && !panicPicture.Equals("") && !File.Exists(panicPicture))
                {
                    warnings.Add("The panic picture you entered could not be found.");
                }

                // Validate API limit options
                bool cycleOldImages = cycleOldImagesCheckBox.Checked;
                int oldImageLimit = (int) oldImageLimitNumericUpDown.Value;
                if (cycleOldImages && oldImageLimit == 0 && visitedApiTab && visitedBehaviorTab)
                {
                    warnings.Add("You have chosen to use previously downloaded pictures when your API limit is reached, but you have chosen to delete all old pictures as soon as you download a new one.");
                }

                // Validate actions
                bool changeWallpaper = changeWallpaperCheckBox.Checked;
                bool changeLockScreen = changeLockScreenCheckBox.Checked;
                if (!changeWallpaper && !changeLockScreen && visitedBehaviorTab)
                {
                    warnings.Add("You have chosen not to change your wallpaper or lock screen background. Exact Ferret will run, but the image will only be saved to your computer.");
                }

                // Validate search domain
                string searchDomain = searchDomainTextBox.Text;
                if (searchDomain.Contains("/"))
                {
                    warnings.Add("You have entered a URL for the search domain. It is recommended that you use only a simple domain name, for example: ef.kangaroostandard.com");
                }

                // Validate minimum and maximum sizes
                int minimumImageWidth = (int)minimumWidthUpDown.Value;
                int maximumImageWidth = (int)maximumWidthUpDown.Value;
                if (minimumImageWidth > maximumImageWidth)
                {
                    warnings.Add("Your minimum image width is greater than the maximum.");
                }
                int minimumImageHeight = (int)minimumHeightUpDown.Value;
                int maximumImageHeight = (int)maximumHeightUpDown.Value;
                if (minimumImageHeight > maximumImageHeight)
                {
                    warnings.Add("Your minimum image height is greater than the maximum.");
                }

                // Validate minimum and maximum ratios
                double minimumImageWHRatio = (double)minimumImageRatioUpDown.Value;
                double maximumImageWHRatio = (double)maximumImageRatioUpDown.Value;
                if (minimumImageWHRatio > maximumImageWHRatio && visitedSearchTab)
                {
                    warnings.Add("Your minimum image width:height ratio is greater than the maximum.");
                }

                // Validate size and ratio together
                double minimumPossibleRatio = (double)minimumImageWidth / maximumImageHeight;
                double maximumPossibleRatio = (double)maximumImageWidth / minimumImageHeight;
                if (minimumPossibleRatio > maximumImageWHRatio)
                {
                    warnings.Add("Your minimum image width:height ratio is impossible to achieve based on your minimum and maximum width and height.");
                }
                if (maximumPossibleRatio < minimumImageWHRatio)
                {
                    warnings.Add("Your maximum image width:height ratio is impossible to achieve based on your minimum and maximum width and height.");
                }

                // Validate API key
                string googleApiKey = googleApiKeyTextBox.Text;
                string bingAccessToken = bingAccessTokenTextBox.Text;
                if (googleApiKey.Equals("") && bingAccessToken.Equals("") && visitedApiTab)
                {
                    warnings.Add("You have not entered a Google Custom Search API key or a Bing access token. Exact Ferret will not work without one of these.");
                }

                // Validate log level
                int logLevel = logLevelComboBox.SelectedIndex;
                if (logLevel == Logging.Log.SPECIAL)
                {
                    warnings.Add("You have chosen to log raw search results. It is recommended that you use this option sparingly because of the sheer amount of data it generates.");
                }

                // Validate interval
                int intervalMinutes = (int)intervalNumericUpDown.Value;
                int intervalSeconds = (int)intervalSecondsNumericUpDown.Value;
                if (intervalMinutes == 0 && intervalSeconds <= 15 && visitedProcessTab)
                {
                    warnings.Add("You have chosen to run Exact Ferret every 0 minutes and " + intervalSeconds + " seconds. This could possibly slow down your system.");
                }

                foreach (string warning in warnings)
                {
                    DialogResult result = MessageBox.Show(this, warning + "\r\nDo you want to save anyway?",
                            "Exact Ferret", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        return false;
                    }
                }

                // Actually save the settings
                PropertiesManager.save();

                // If the service is started, restart it
                if (Communication.ping(0))
                {
                    stopExactFerret(null, null);
                    if (restartNoDelay)
                        startExactFerret(null, null);
                    else
                        startExactFerretWithDelay();
                }

                if (Communication.ping(1))
                    Communication.stopExactFerret(1);
                if (PropertiesManager.getEnableDesktopLabel())
                    Communication.startExactFerret(1);
            }

            return true;
        }

        private void changeWallpaperCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            wallpaperStyleComboBox.Enabled = changeWallpaperCheckBox.Checked;
            SetDirtyBit(sender, e);
        }

        private void dictionaryFileBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.InitialDirectory = PropertiesManager.getInstallationDirectoryPath() + "\\Dictionaries";
            Dialog.DefaultExt = "txt";
            Dialog.Filter = "Text files|*.txt|All files|*";
            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                dictionaryFileTextBox.Text = Dialog.FileName;
        }

        private void exportSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportForm exportForm = new ExportForm();
            DialogResult result = exportForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] propertyNames = exportForm.getPropertyNames();

                SaveFileDialog Dialog = new SaveFileDialog();
                Dialog.DefaultExt = "exf";
                Dialog.Filter = "Exact Ferret Settings Files|*.exf";
                if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PropertiesManager.exportSettings(Dialog.FileName, propertyNames);
                }
            }
        }

        private void importSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.DefaultExt = "exf";
            Dialog.Filter = "Exact Ferret Settings Files|*.exf";
            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (PropertiesManager.importSettings(Dialog.FileName))
                {
                    
                    freezeCustomSettingControls = true;

                    // Update start at logon checkbox
                    startOnLogOnCheckBox.Checked = PropertiesManager.getStartAtLogonEnabled();

                    // Update enable desktop label checkbox
                    enableDesktopLabelCheckBox.Checked = PropertiesManager.getEnableDesktopLabel();

                    // Update shortcut dropdowns
                    quickRunShortcutComboBox.SelectedIndex = PropertiesManager.getQuickRunShortcut();
                    panicShortcutComboBox.SelectedIndex = PropertiesManager.getPanicShortcut();
                    settingsShortcutComboBox.SelectedIndex = PropertiesManager.getSettingsShortcut();

                    freezeCustomSettingControls = false;

                    // Update list boxes bound to data sources
                    ((BindingSource)blockedImagesList.DataSource).ResetBindings(false);
                    ((BindingSource)blockedDomainsList.DataSource).ResetBindings(false);
                    ((BindingSource)searchEngineOrderListBox.DataSource).ResetBindings(false);
                }
                else
                {
                    MessageBox.Show(this, "The settings file you are trying to import is not in the correct format.", 
                            "Exact Ferret", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void stopExactFerret(object sender, EventArgs e)
        {
            bool success = Communication.stopExactFerret(0);
            if (!success)
                MessageBox.Show(this, "Could not stop the Exact Ferret background process.", "Exact Ferret", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void startExactFerret(object sender, EventArgs e)
        {
            if (askToSave("starting the background process", true) == DialogResult.Cancel)
                return;

            Communication.startExactFerret(0);
            if (!Communication.ping(1) && PropertiesManager.getEnableDesktopLabel())
                Communication.startExactFerret(1);
        }

        public void pingResponse(bool success)
        {
            if (success)
            {
                // Process is started, so disable the Start button
                SetControlPropertyValue(startButton, "Enabled", false);
                SetControlPropertyValue(stopButton, "Enabled", true);
                SetControlPropertyValue(statusLabel, "Text", "Started");

                // Also get the current countdown
                int countdownSeconds = Communication.getCountdown();
                if (countdownSeconds >= 0)
                {
                    string countdownString = TimeSpan.FromSeconds(countdownSeconds).ToString();
                    SetControlPropertyValue(countdownLabel, "Text", "Next picture in " + countdownString);
                }
            }
            else
            {
                // Process is stopped, so enable the Start button
                SetControlPropertyValue(startButton, "Enabled", true);
                SetControlPropertyValue(stopButton, "Enabled", false);
                SetControlPropertyValue(statusLabel, "Text", "Stopped");
                SetControlPropertyValue(countdownLabel, "Text", "");
            }
        }

        delegate void SetControlValueCallback(Control oControl, string propName, object propValue);
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SetControlPropertyValue(Control oControl, string propName, object propValue)
        {
            if (oControl.InvokeRequired)
            {
                SetControlValueCallback d = new SetControlValueCallback(SetControlPropertyValue);
                oControl.BeginInvoke(d, new object[] { oControl, propName, propValue });
            }
            else
            {
                Type t = oControl.GetType();
                PropertyInfo[] props = t.GetProperties();
                foreach (PropertyInfo p in props)
                {
                    if (p.Name.ToUpper() == propName.ToUpper())
                    {
                        p.SetValue(oControl, propValue, null);
                    }
                }
            }
        }

        private void commitOtherSettings()
        {
            // Start at logon
            PropertiesManager.setStartAtLogin(startOnLogOnCheckBox.Checked);

            // Enable desktop label
            PropertiesManager.setEnableDesktopLabel(enableDesktopLabelCheckBox.Checked);

            // Shortcuts
            PropertiesManager.setQuickRunShortcut(quickRunShortcutComboBox.SelectedIndex);
            PropertiesManager.setPanicShortcut(panicShortcutComboBox.SelectedIndex);
            PropertiesManager.setSettingsShortcut(settingsShortcutComboBox.SelectedIndex);
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpFileLocation = PropertiesManager.HELP_FILE_PATH;
            Process.Start(helpFileLocation);
        }

        private void openLogButton_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = PropertiesManager.getLogFilePath();
            try
            {
                process.Start();
            }
            catch (Exception e1)
            {
                MessageBox.Show(this, "Could not find the log file.", "Exact Ferret", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void quickRunButton_Click(object sender, EventArgs e)
        {
            bool alreadyStarted = Communication.ping(0);
            DialogResult result = askToSave("downloading a new picture", true);
            if (result == DialogResult.Cancel || (result == DialogResult.Yes && alreadyStarted && PropertiesManager.isScheduledNow()))
                return;

            disablePictureButtons(false);

            string path = PropertiesManager.getInstallationDirectoryPath() + 
                    "\\" + PropertiesManager.EXACT_FERRET_EXECUTABLE_NAME;
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = "-quick";
            process.Start();

            Thread thread = new Thread(new ThreadStart(() => waitForQuickRun(process)));
            threadsRunning++;
            thread.Start();
        }

        private void waitForQuickRun(Process quickRunProcess)
        {
            quickRunProcess.WaitForExit();
            lockPictureButtons = false;
            updatePictureButtons();
            threadsRunning--;
        }

        private void updateImagePreviewLoop()
        {
            while (true)
            {
                updateImagePreview();
                Thread.Sleep(IMAGE_PREVIEW_UPDATE_INTERVAL_MILLISECONDS);
                if (exiting)
                    break;
            }
            threadsRunning--;
        }

        private void updateImagePreview()
        {
            // Set the picture in the image preview box
            string picturePath = Desktop.getCurrentPicturePath();

            if (picturePath != null)
                try
                {
                    Image image;
                    lock (imagePreviewKey)
                    {
                        using (Stream pictureStream = File.Open(picturePath, FileMode.Open))
                        {
                            image = Image.FromStream(pictureStream);
                        }
                    }
                    SetControlPropertyValue(imagePreviewBox, "Image", image);

                    string pictureDomain = PictureFileManager.getDomainFromPicturePath(picturePath);
                    if (pictureDomain != null)
                        SetControlPropertyValue(imageDomainLabel, "Text", "Downloaded from " + pictureDomain);
                    else
                        SetControlPropertyValue(imageDomainLabel, "Text", "");

                    string searchTerm = PictureFileManager.getSearchTermFromPicturePath(picturePath);
                    if (searchTerm != null)
                        SetControlPropertyValue(searchTermLabel, "Text", "Searched \"" + searchTerm + "\"");
                    else
                        SetControlPropertyValue(searchTermLabel, "Text", "");

                    string caption = PictureFileManager.getCaptionFromPicturePath(picturePath);
                    if (caption != null)
                        SetControlPropertyValue(captionLabel, "Text", caption);
                    else
                        SetControlPropertyValue(captionLabel, "Text", "");
                }
                catch (Exception e)
                {
                    SetControlPropertyValue(imagePreviewBox, "Image", null);
                }
            else
                SetControlPropertyValue(imagePreviewBox, "Image", null);

            updatePictureButtons();
        }

        private DialogResult askToSave(string thingThatWillBeHappening, bool restartNoDelay)
        {
            if (PropertiesManager.dirtyFlag)
            {
                DialogResult result = MessageBox.Show(this, "Do you want to save changes to Exact Ferret settings before " + 
                        thingThatWillBeHappening + "?", "Exact Ferret", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (!saveAllSettings(restartNoDelay))
                        return DialogResult.Cancel;
                    else
                        return result;
                }
                else
                {
                    return result;
                }
            }

            return DialogResult.None;
        }

        private void disablePictureButtons(bool isForMissingPicture)
        {
            SetControlPropertyValue(pictureBackButton, "Enabled", false);
            SetControlPropertyValue(pictureNextButton, "Enabled", false);
            SetControlPropertyValue(deleteButton, "Enabled", false);
            
            if (isForMissingPicture)
            {
                string latestPicture = Desktop.getLatestPicturePath();
                if (latestPicture == null || !File.Exists(latestPicture))
                {
                    SetControlPropertyValue(pictureCurrentButton, "Enabled", false);
                }
            }
            else
            {
                SetControlPropertyValue(quickRunButton, "Enabled", false);
                SetControlPropertyValue(searchButton, "Enabled", false);
                SetControlPropertyValue(pictureCurrentButton, "Enabled", false);
                lockPictureButtons = true;
            }
        }

        private void navigateToPicture(string term)
        {
            disablePictureButtons(false);
            
            // Get the path to the current picture
            string currentPicture = Desktop.getCurrentPicturePath();
            if (currentPicture == null && !term.Equals("current"))
                return;

            List<FileInfo> imageFiles = PictureFileManager.getPicturesListSortedByTimestamp();

            string newPicture = null;
            if (term == "current")
            {
                newPicture = imageFiles[imageFiles.Count - 1].FullName;
            }
            else 
            {
                // Find the current picture in the list and set the appropriate one
                for (int i = imageFiles.Count - 1; i >= 0; i--)
                {
                    if (imageFiles[i].FullName.Equals(currentPicture))
                    {
                        switch (term)
                        {
                            case "previous":
                                if (i == 0)
                                    newPicture = null;
                                else
                                    newPicture = imageFiles[i - 1].FullName;
                                break;
                            case "next":
                                if (i == imageFiles.Count - 1)
                                    newPicture = null;
                                else
                                    newPicture = imageFiles[i + 1].FullName;
                                break;
                            default:
                                newPicture = null;
                                break;
                        }
                    }
                }
            }
            threadsRunning++;
            new Thread(new ThreadStart(() => {
                Desktop.changePicture(newPicture);
                lockPictureButtons = false;
                updatePictureButtons();
                threadsRunning--;
            })).Start();
        }

        private void updatePictureButtons()
        {
            if (lockPictureButtons)
                return;

            bool previousEnabled = false, nextEnabled = false;

            // Get the path to the current picture
            string currentPicture = Desktop.getCurrentPicturePath();
            if (currentPicture == null || !File.Exists(currentPicture))
            {
                disablePictureButtons(true);
            }
            else
            {
                List<FileInfo> imageFiles = PictureFileManager.getPicturesListSortedByTimestamp();

                if (imageFiles != null)
                {
                    // Find the current picture in the list
                    previousEnabled = true;
                    nextEnabled = true;
                    for (int i = imageFiles.Count - 1; i >= 0; i--)
                    {
                        if (imageFiles[i].FullName.Equals(currentPicture))
                        {
                            if (i == 0)
                                previousEnabled = false;
                            if (i == imageFiles.Count - 1)
                                nextEnabled = false;

                            break;
                        }
                    }
                }

                SetControlPropertyValue(deleteButton, "Enabled", true);
            }

            SetControlPropertyValue(pictureBackButton, "Enabled", previousEnabled);
            SetControlPropertyValue(pictureNextButton, "Enabled", nextEnabled);
            SetControlPropertyValue(quickRunButton, "Enabled", true);
            SetControlPropertyValue(searchButton, "Enabled", true);

            string latestPicture = Desktop.getLatestPicturePath();
            if (latestPicture != null && File.Exists(latestPicture))
                SetControlPropertyValue(pictureCurrentButton, "Enabled", true);
        }

        private void pictureBackButtonClick(object sender, EventArgs e)
        {
            // Go the the previous picture
            navigateToPicture("previous");
        }

        private void pictureNextButtonClick(object sender, EventArgs e)
        {
            // Go the the next picture
            navigateToPicture("next");
        }

        private void pictureCurrentButtonClick(object sender, EventArgs e)
        {
            // Go the the current picture
            navigateToPicture("current");
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            deleteButton.ContextMenuStrip.Show(deleteButton, deleteButton.PointToClient(Cursor.Position));
        }

        private string deleteCurrentPicture()
        {
            string currentPicture = Desktop.getCurrentPicturePath();
            if (!File.Exists(currentPicture))
                return null;
            
            // Find a suitable picture to go to
            if (pictureBackButton.Enabled)
            {
                pictureBackButtonClick(null, null);
            }
            else if (pictureNextButton.Enabled)
            {
                pictureNextButtonClick(null, null);
            }

            lock (imagePreviewKey)
            {
                PictureFileManager.deleteFile(currentPicture);
            }

            return currentPicture;
        }

        private void neverShowMeThisPictureAgainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string imageUrl = PictureFileManager.getUrlFromPicturePath(Desktop.getCurrentPicturePath());
            PropertiesManager.blockImage(imageUrl);
            ((BindingSource)blockedImagesList.DataSource).ResetBindings(false);
            
            deleteCurrentPicture();
        }

        private void neverShowMeAnyPictureFromThisDomainAgainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string imageDomain = PictureFileManager.getDomainFromPicturePath(Desktop.getCurrentPicturePath());
            PropertiesManager.blockDomain(imageDomain);
            ((BindingSource)blockedDomainsList.DataSource).ResetBindings(false);

            deleteCurrentPicture();
        }

        private void unblockImageButton_Click(object sender, EventArgs e)
        {
            List<string> selectedImages = CollectionUtil.convertSelectedObjectsCollectionToStringList(
                    blockedImagesList.SelectedItems);
            PropertiesManager.unblockImages(selectedImages);
            ((BindingSource)blockedImagesList.DataSource).ResetBindings(false);
        }

        private void unblockDomainButton_Click(object sender, EventArgs e)
        {
            List<string> selectedDomains = CollectionUtil.convertSelectedObjectsCollectionToStringList(
                    blockedDomainsList.SelectedItems);
            PropertiesManager.unblockDomains(selectedDomains);
            ((BindingSource)blockedDomainsList.DataSource).ResetBindings(false);
        }

        private void startOnLogOnCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!freezeCustomSettingControls)
                PropertiesManager.setStartAtLogin(startOnLogOnCheckBox.Checked);
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(NumericUpDown))
            {
                NumericUpDown n = (NumericUpDown)sender;
                n.Select(0, n.Text.Length);
            }
        }

        private void getAPIKeyButton_Click(object sender, EventArgs e)
        {
            string url = "https://console.developers.google.com/project";
            Process.Start(url);
        }

        private void wallpaperStyleComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetDirtyBit(sender, e);
            
            // Change the current wallpaper's style
            ComboBox wallpaperStyleComboBox = (ComboBox)sender;
            PropertiesManager.setWallpaperStyle(wallpaperStyleComboBox.SelectedIndex);
            Thread t = new Thread(new ThreadStart(() =>
            {
                bool success = Desktop.changePicture(Desktop.getCurrentPicturePath());
                if (!success)
                {
                    Thread.Sleep(2000);
                    Desktop.changePicture(Desktop.getCurrentPicturePath());
                }
                threadsRunning--;
            }));
            threadsRunning++;
            t.Start();
            t.Join(TimeSpan.FromSeconds(5));
        }

        private void pingLoop(Action<bool> consumerFunction)
        {
            while (true)
            {
                consumerFunction(Communication.ping(0));
                Thread.Sleep(Communication.PING_INTERVAL_MILLISECONDS);
                if (exiting)
                    break;
            }
            threadsRunning--;
        }

        private void scheduleRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            scheduleBox.Enabled = scheduleRadioButton.Checked;
        }

        private void allTheTimeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (allTheTimeRadioButton.Checked)
            {
                PropertiesManager.setScheduleAllTheTime();

                sundayScheduleCheckBox.Checked = true;
                mondayScheduleCheckBox.Checked = true;
                tuesdayScheduleCheckBox.Checked = true;
                wednesdayScheduleCheckBox.Checked = true;
                thursdayScheduleCheckBox.Checked = true;
                fridayScheduleCheckBox.Checked = true;
                saturdayScheduleCheckBox.Checked = true;
            }
        }

        private void sundayScheduleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            sundayScheduleStartPicker.Enabled = sundayScheduleCheckBox.Checked;
            sundayScheduleStopPicker.Enabled = sundayScheduleCheckBox.Checked;
            if (!sundayScheduleCheckBox.Checked)
                PropertiesManager.setSundayScheduleDisabled();
            else
                PropertiesManager.setSundayScheduleAllDay();
        }

        private void mondayScheduleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mondayScheduleStartPicker.Enabled = mondayScheduleCheckBox.Checked;
            mondayScheduleStopPicker.Enabled = mondayScheduleCheckBox.Checked;
            if (!mondayScheduleCheckBox.Checked)
                PropertiesManager.setMondayScheduleDisabled();
            else
                PropertiesManager.setMondayScheduleAllDay();
        }

        private void tuesdayScheduleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            tuesdayScheduleStartPicker.Enabled = tuesdayScheduleCheckBox.Checked;
            tuesdayScheduleStopPicker.Enabled = tuesdayScheduleCheckBox.Checked;
            if (!tuesdayScheduleCheckBox.Checked)
                PropertiesManager.setTuesdayScheduleDisabled();
            else
                PropertiesManager.setTuesdayScheduleAllDay();
        }

        private void wednesdayScheduleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            wednesdayScheduleStartPicker.Enabled = wednesdayScheduleCheckBox.Checked;
            wednesdayScheduleStopPicker.Enabled = wednesdayScheduleCheckBox.Checked;
            if (!wednesdayScheduleCheckBox.Checked)
                PropertiesManager.setWednesdayScheduleDisabled();
            else
                PropertiesManager.setWednesdayScheduleAllDay();
        }

        private void thursdayScheduleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            thursdayScheduleStartPicker.Enabled = thursdayScheduleCheckBox.Checked;
            thursdayScheduleStopPicker.Enabled = thursdayScheduleCheckBox.Checked;
            if (!thursdayScheduleCheckBox.Checked)
                PropertiesManager.setThursdayScheduleDisabled();
            else
                PropertiesManager.setThursdayScheduleAllDay();
        }

        private void fridayScheduleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            fridayScheduleStartPicker.Enabled = fridayScheduleCheckBox.Checked;
            fridayScheduleStopPicker.Enabled = fridayScheduleCheckBox.Checked;
            if (!fridayScheduleCheckBox.Checked)
                PropertiesManager.setFridayScheduleDisabled();
            else
                PropertiesManager.setFridayScheduleAllDay();
        }

        private void saturdayScheduleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            saturdayScheduleStartPicker.Enabled = saturdayScheduleCheckBox.Checked;
            saturdayScheduleStopPicker.Enabled = saturdayScheduleCheckBox.Checked;
            if (!saturdayScheduleCheckBox.Checked)
                PropertiesManager.setSaturdayScheduleDisabled();
            else
                PropertiesManager.setSaturdayScheduleAllDay();
        }

        private void scheduleSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScheduleSettingsForm form = new ScheduleSettingsForm();
            DialogResult result = form.ShowDialog();
        }

        private void updateFormFromScheduleLoop()
        {
            updateFormFromSchedule();
            int millsElapsed = 0;
            while (true)
            {
                Thread.Sleep(1000);
                millsElapsed += 1000;
                if (millsElapsed >= SCHEDULE_FORM_UPDATE_INTERVAL_MILLISECONDS)
                {
                    updateFormFromSchedule();
                    millsElapsed = 0;
                }
                if (exiting)
                    break;
            }
            threadsRunning--;
        }

        private void updateFormFromSchedule()
        {
            removeAllFormHighlighting();

            SettingsSchedule[] schedules = SettingsSchedule.getScheduledSettings();
            foreach (SettingsSchedule schedule in schedules)
            {
                Hashtable propertiesInFile = PropertiesManager.getPropertiesFromSettingsFile(schedule.file.info.FullName);
                if (propertiesInFile != null)
                {
                    foreach (string propertyName in propertiesInFile.Keys)
                    {
                        highlightFormByPropertyName(propertyName, propertiesInFile[propertyName], schedule.ToString());
                    }
                }
            }
        }

        private void removeAllFormHighlighting()
        {
            foreach (Control control in new Control[]{
                intervalNumericUpDown, changeLockScreenCheckBox, cycleOldImagesCheckBox, maximumImageRatioUpDown, 
                colorsInImageComboBox, safeSearchCheckBox, fridayScheduleStartPicker, intervalSecondsNumericUpDown, 
                logLevelComboBox, wallpaperStyleComboBox, blockedImagesList, saturdayScheduleStartPicker, 
                searchDomainTextBox, minimumHeightUpDown, logRetentionDaysNumericUpDown, wednesdayScheduleStopPicker, 
                fridayScheduleStopPicker, typeOfImageComboBox, mondayScheduleStartPicker, thursdayScheduleStartPicker, 
                logFileLocationTextBox, imageLocationTextBox, apiLimitWarningCheckBox, searchTermModifierTextBox, 
                sundayScheduleStopPicker, maximumHeightUpDown, tuesdayScheduleStartPicker, minimumImageRatioUpDown, 
                tuesdayScheduleStopPicker, maximumWidthUpDown, saturdayScheduleStopPicker, autoUpdateCheckbox, 
                blockedDomainsList, httpRequestTimeoutNumericUpDown, dictionaryFileTextBox, oldImageLimitNumericUpDown, 
                wednesdayScheduleStartPicker, downloadAttemptsNumericUpDown, mondayScheduleStopPicker, minimumWidthUpDown, 
                thursdayScheduleStopPicker, sundayScheduleStartPicker, googleApiKeyTextBox, logRolloverKBytesNumericUpDown, 
                changeWallpaperCheckBox, startOnLogOnCheckBox, dontRunOnBatteryPowerCheckBox, singleApiLimitWarningCheckBox, 
                searchEngineOrderListBox, bingAccessTokenTextBox
            })
            {
                if (control.BackColor == SystemColors.MenuHighlight)
                {
                    SetControlPropertyValue(control, "BackColor", SystemColors.Window);
                    SetToolTipValue(toolTip1, control, savedTooltips[control].ToString());
                }
            }
        }

        private void highlightFormByPropertyName(string propertyName, object newValue, string settingsFileName)
        {
            Control control = null;
            switch (propertyName)
            {
                case "CYCLE_AT_API_LIMIT":
                    control = cycleOldImagesCheckBox;
                    break;
                case "MAXIMUM_IMAGE_RATIO":
                    control = maximumImageRatioUpDown;
                    break;
                case "COLORS_IN_IMAGE":
                    control = colorsInImageComboBox;
                    break;
                case "SAFE_SEARCH":
                    control = safeSearchCheckBox;
                    break;
                case "INTERVAL_MINUTES":
                    control = intervalNumericUpDown;
                    break;
                case "FRIDAY_START":
                    control = fridayScheduleStartPicker;
                    break;
                case "INTERVAL_SECONDS":
                    control = intervalSecondsNumericUpDown;
                    break;
                case "LOG_LEVEL":
                    control = logLevelComboBox;
                    break;
                case "WALLPAPER_STYLE":
                    control = wallpaperStyleComboBox;
                    break;
                case "BLOCKED_IMAGES":
                    control = blockedImagesList;
                    break;
                case "SATURDAY_START":
                    control = saturdayScheduleStartPicker;
                    break;
                case "SEARCH_DOMAIN":
                    control = searchDomainTextBox;
                    break;
                case "MINIMUM_IMAGE_HEIGHT":
                    control = minimumHeightUpDown;
                    break;
                case "LOG_RETENTION_DAYS":
                    control = logRetentionDaysNumericUpDown;
                    break;
                case "WEDNESDAY_STOP":
                    control = wednesdayScheduleStopPicker;
                    break;
                case "FRIDAY_STOP":
                    control = fridayScheduleStopPicker;
                    break;
                case "TYPE_OF_IMAGE":
                    control = typeOfImageComboBox;
                    break;
                case "MONDAY_START":
                    control = mondayScheduleStartPicker;
                    break;
                case "THURSDAY_START":
                    control = thursdayScheduleStartPicker;
                    break;
                case "LOG_FILE_LOCATION":
                    control = logFileLocationTextBox;
                    break;
                case "IMAGE_LOCATION":
                    control = imageLocationTextBox;
                    break;
                case "API_LIMIT_WARNING":
                    control = apiLimitWarningCheckBox;
                    break;
                case "SEARCH_TERM_MODIFIER":
                    control = searchTermModifierTextBox;
                    break;
                case "SUNDAY_STOP":
                    control = sundayScheduleStopPicker;
                    break;
                case "MAXIMUM_IMAGE_HEIGHT":
                    control = maximumHeightUpDown;
                    break;
                case "TUESDAY_START":
                    control = tuesdayScheduleStartPicker;
                    break;
                case "MINIMUM_IMAGE_RATIO":
                    control = minimumImageRatioUpDown;
                    break;
                case "TUESDAY_STOP":
                    control = tuesdayScheduleStopPicker;
                    break;
                case "MAXIMUM_IMAGE_WIDTH":
                    control = maximumWidthUpDown;
                    break;
                case "SATURDAY_STOP":
                    control = saturdayScheduleStopPicker;
                    break;
                case "AUTO_UPDATE":
                    control = autoUpdateCheckbox;
                    break;
                case "BLOCKED_DOMAINS":
                    control = blockedDomainsList;
                    break;
                case "HTTP_REQUEST_TIMEOUT":
                    control = httpRequestTimeoutNumericUpDown;
                    break;
                case "DICTIONARY_FILE":
                    control = dictionaryFileTextBox;
                    break;
                case "MAX_OLD_IMAGES":
                    control = oldImageLimitNumericUpDown;
                    break;
                case "WEDNESDAY_START":
                    control = wednesdayScheduleStartPicker;
                    break;
                case "CHANGE_LOCK_SCREEN":
                    control = changeLockScreenCheckBox;
                    break;
                case "DOWNLOAD_ATTEMPTS":
                    control = downloadAttemptsNumericUpDown;
                    break;
                case "MONDAY_STOP":
                    control = mondayScheduleStopPicker;
                    break;
                case "MINIMUM_IMAGE_WIDTH":
                    control = minimumWidthUpDown;
                    break;
                case "THURSDAY_STOP":
                    control = thursdayScheduleStopPicker;
                    break;
                case "SUNDAY_START":
                    control = sundayScheduleStartPicker;
                    break;
                case "GOOGLE_API_KEY":
                    control = googleApiKeyTextBox;
                    break;
                case "LOG_ROLLOVER_KBYTES":
                    control = logRolloverKBytesNumericUpDown;
                    break;
                case "CHANGE_WALLPAPER":
                    control = changeWallpaperCheckBox;
                    break;
                case "startAtLogon":
                    control = startOnLogOnCheckBox;
                    break;
                case "DONT_RUN_ON_BATTERY":
                    control = dontRunOnBatteryPowerCheckBox;
                    break;
                case "BING_ACCESS_TOKEN":
                    control = bingAccessTokenTextBox;
                    break;
                case "SEARCH_ENGINE_ORDER":
                    control = searchEngineOrderListBox;
                    break;
                case "SINGLE_API_LIMIT_WARNING":
                    control = singleApiLimitWarningCheckBox;
                    break;
                //default:
                    //throw new Exception("Control not found for property name " + propertyName);
            }

            if (control != null)
            {
                SetControlPropertyValue(control, "BackColor", SystemColors.MenuHighlight);
                try
                {
                    savedTooltips.Add(control, toolTip1.GetToolTip(control));
                }
                catch { }
                
                if (control.GetType() == typeof(ComboBox))
                    newValue = ((ComboBox)control).Items[int.Parse(newValue.ToString())];
                else if (control.GetType() == typeof(DateTimePicker))
                    newValue = new DateTime(long.Parse(newValue.ToString())).TimeOfDay;

                SetToolTipValue(toolTip1, control, "Overriden by " + settingsFileName + " to " + newValue);
            }
        }

        delegate void SetToolTipValueCallback(ToolTip toolTip, Control control, string toopTipText);
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SetToolTipValue(ToolTip toolTip, Control control, string toolTipText)
        {
            if (control.InvokeRequired)
            {
                SetToolTipValueCallback d = new SetToolTipValueCallback(SetToolTipValue);
                control.BeginInvoke(d, new object[] { toolTip, control, toolTipText });
            }
            else
            {
                toolTip.SetToolTip(control, toolTipText);
            }
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            processEvents = true;
            MessageBox.Show("mouse");
        }

        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            processEvents = true;
            MessageBox.Show("key:;");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string url = "https://datamarket.azure.com/dataset/5BA839F1-12CE-4CCE-BF57-A49D98D29A44";
            Process.Start(url);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            int selectedIndex = searchEngineOrderListBox.SelectedIndex;
            if (selectedIndex >= 1)
            {
                string selectedEngine = searchEngineOrderListBox.SelectedItem.ToString();

                StringCollection searchEngineOrder = new StringCollection();
                string[] searchEngineOrderArray = new string[searchEngineOrderListBox.Items.Count];
                searchEngineOrderListBox.Items.CopyTo(searchEngineOrderArray, 0);
                searchEngineOrder.AddRange(searchEngineOrderArray);

                searchEngineOrder.RemoveAt(selectedIndex);
                searchEngineOrder.Insert(selectedIndex - 1, selectedEngine);

                PropertiesManager.setSearchEngineOrder(searchEngineOrder);
                BindingSource searchEngineOrderSource = new BindingSource();
                searchEngineOrderSource.DataSource = PropertiesManager.getSearchEngineOrderDataSource();
                searchEngineOrderListBox.DataSource = searchEngineOrderSource;
                ((BindingSource)searchEngineOrderListBox.DataSource).ResetBindings(false);

                searchEngineOrderListBox.SelectedIndex = selectedIndex - 1;
            }
        }

        private void searchEngineOrderDownButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = searchEngineOrderListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < searchEngineOrderListBox.Items.Count - 1)
            {
                string selectedEngine = searchEngineOrderListBox.SelectedItem.ToString();

                StringCollection searchEngineOrder = new StringCollection();
                string[] searchEngineOrderArray = new string[searchEngineOrderListBox.Items.Count];
                searchEngineOrderListBox.Items.CopyTo(searchEngineOrderArray, 0);
                searchEngineOrder.AddRange(searchEngineOrderArray);
                
                searchEngineOrder.RemoveAt(selectedIndex);
                searchEngineOrder.Insert(selectedIndex + 1, selectedEngine);

                PropertiesManager.setSearchEngineOrder(searchEngineOrder);
                BindingSource searchEngineOrderSource = new BindingSource();
                searchEngineOrderSource.DataSource = PropertiesManager.getSearchEngineOrderDataSource();
                searchEngineOrderListBox.DataSource = searchEngineOrderSource;
                ((BindingSource)searchEngineOrderListBox.DataSource).ResetBindings(false);

                searchEngineOrderListBox.SelectedIndex = selectedIndex + 1;
            }
        }

        private void searchEngineOrderListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = searchEngineOrderListBox.SelectedIndex;
            searchEngineOrderUpButton.Enabled = selectedIndex >= 1;
            searchEngineOrderDownButton.Enabled = selectedIndex >= 0 && selectedIndex < searchEngineOrderListBox.Items.Count - 1;
        }

        private void quickRunShortcutComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!freezeCustomSettingControls)
                PropertiesManager.setQuickRunShortcut(quickRunShortcutComboBox.SelectedIndex);
        }

        private void panicShortcutComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!freezeCustomSettingControls)
                PropertiesManager.setPanicShortcut(panicShortcutComboBox.SelectedIndex);
        }

        private void settingsShortcutComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!freezeCustomSettingControls)
                PropertiesManager.setSettingsShortcut(settingsShortcutComboBox.SelectedIndex);
        }

        private void panicRemoveRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (panicRemoveRadioButton.Checked)
            {
                panicSwitchTextBox.Text = "";
            }
        }

        private void panicSwitchRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            panicSwitchTextBox.Enabled = panicSwitchRadioButton.Checked;
            panicImageBrowseButton.Enabled = panicSwitchRadioButton.Checked;
        }

        private void panicImageBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.DefaultExt = "jpg";
            Dialog.Filter = "JPEG|*.jpg;*.jpeg";
            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                panicSwitchTextBox.Text = Dialog.FileName;
        }

        private void panicNowButton_Click(object sender, EventArgs e)
        {
            disablePictureButtons(false);

            string path = PropertiesManager.getInstallationDirectoryPath() +
                    "\\" + PropertiesManager.EXACT_FERRET_EXECUTABLE_NAME;
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = "-panic";
            process.Start();

            Thread thread = new Thread(new ThreadStart(() => waitForPanic(process)));
            threadsRunning++;
            thread.Start();
        }

        private void waitForPanic(Process panicProcess)
        {
            panicProcess.WaitForExit();
            lockPictureButtons = false;
            updatePictureButtons();
            threadsRunning--;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            bool alreadyStarted = Communication.ping(0);
            DialogResult result = askToSave("downloading a new picture", false);
            if (result == DialogResult.Cancel)
                return;

            // Get a search term from the user
            StringInput input = new StringInput("What do you want to find?", "Exact Ferret", lastTermUsedInSearch);
            if (input.ShowDialog() == DialogResult.Cancel)
                return;

            lastTermUsedInSearch = input.UserInput;
            string term = input.UserInput.Replace("\"", "@");

            disablePictureButtons(false);

            string path = PropertiesManager.getInstallationDirectoryPath() +
                    "\\" + PropertiesManager.EXACT_FERRET_EXECUTABLE_NAME;
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = "-quick \"" + term + "\"";
            process.Start();

            Thread thread = new Thread(new ThreadStart(() => waitForQuickRun(process)));
            threadsRunning++;
            thread.Start();
        }

        private void startExactFerretWithDelay()
        {
            Communication.startExactFerretWithDelay();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            processEvents = true;
        }

        private void tab_Enter(object sender, EventArgs e)
        {
            // Make sure the tab actually changed
            if (tabControl1.SelectedIndex != rememberedTabIndex)
            {
                rememberedTabIndex = tabControl1.SelectedIndex;

                switch (tabControl1.SelectedIndex)
                {
                    case 0:
                        if (visitedGeneralTab)
                            processEvents = false;
                        else
                            processEvents = true;
                        visitedGeneralTab = true;
                        break;
                    case 1:
                        visitedBehaviorTab = true;
                        processEvents = false;
                        break;
                    case 2:
                        visitedSearchTab = true;
                        processEvents = false;
                        break;
                    case 3:
                        visitedApiTab = true;
                        processEvents = false;
                        break;
                    case 4:
                        visitedProcessTab = true;
                        processEvents = false;
                        break;
                    case 5:
                        processEvents = false;
                        break;
                    case 6:
                        visitedPanicTab = true;
                        processEvents = false;
                        break;
                    case 7:
                        processEvents = false;
                        break;
                    case 8:
                        visitedShortcutsTab = true;
                        processEvents = false;
                        break;
                    case 9:
                        processEvents = false;
                        break;
                }
            }
        }

        private void desktopLabelLocationComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetDirtyBit(sender, e);
        }

        private void enableDesktopLabelCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!freezeCustomSettingControls)
                PropertiesManager.setEnableDesktopLabel(enableDesktopLabelCheckBox.Checked);
            desktopLabelGroupBox.Enabled = enableDesktopLabelCheckBox.Checked;
        }
    }
}
