using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret.Settings_Classes
{
    public partial class ScheduleSettingsForm : Form
    {
        private const int CERTAIN_TIMES_CONDITION = 0;
        private const int CERTAIN_DAYS_CONDITION = 1;
        private const int CERTAIN_MONTHS_CONDITION = 2;

        private List<SettingsSchedule> schedules;
        private SettingsSchedule selectedSchedule;
        private bool lockForm = false;

        public ScheduleSettingsForm()
        {
            InitializeComponent();
            certainTimesStartPicker.Value = DateTime.Today;
            certainTimesStopPicker.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
            schedules = PropertiesManager.getSettingsSchedules();
            updateForm(false, null);
        }

        private void updateForm(bool selectAndBox, SettingsSchedule scheduleToSelect)
        {
            andBox.Items.Clear();
            orBox.Items.Clear();
            conditionBox.SelectedIndex = -1;
            certainDaysBox.Visible = false;
            certainMonthsBox.Visible = false;
            certainTimesBox.Visible = false;

            foreach (SettingsSchedule schedule in schedules)
            {
                if (schedule.and)
                {
                    andBox.Items.Add(schedule);
                }
                else
                {
                    orBox.Items.Add(schedule);
                }
            }

            if (selectAndBox)
                andBox.SelectedItem = scheduleToSelect;
            else
                orBox.SelectedItem = scheduleToSelect;
            if (scheduleToSelect == null)
                selectedSchedule = null;

            if (selectedSchedule == null)
            {
                conditionBox.Enabled = false;
                andRemoveButton.Enabled = false;
                orRemoveButton.Enabled = false;
            }

            updateFormCondition();
        }

        private void conditionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (conditionBox.SelectedIndex)
            {
                case CERTAIN_TIMES_CONDITION:
                    certainDaysBox.Visible = false;
                    certainMonthsBox.Visible = false;
                    certainTimesBox.Visible = true;
                    break;
                case CERTAIN_DAYS_CONDITION:
                    certainDaysBox.Visible = true;
                    certainMonthsBox.Visible = false;
                    certainTimesBox.Visible = false;
                    break;
                case CERTAIN_MONTHS_CONDITION:
                    certainDaysBox.Visible = false;
                    certainMonthsBox.Visible = true;
                    certainTimesBox.Visible = false;
                    break;
            }

            updateCondition();
        }

        private void updateCondition()
        {
            if (selectedSchedule == null || lockForm)
                return;

            switch (conditionBox.SelectedIndex)
            {
                case CERTAIN_TIMES_CONDITION:
                    CertainTimesCondition timesCondition;
                    if (selectedSchedule.condition.GetType() != typeof(CertainTimesCondition))
                        timesCondition = new CertainTimesCondition();
                    else
                        timesCondition = (CertainTimesCondition) selectedSchedule.condition;

                    timesCondition.startTime = certainTimesStartPicker.Value.TimeOfDay;
                    timesCondition.stopTime = certainTimesStopPicker.Value.TimeOfDay;
                    selectedSchedule.condition = timesCondition;
                    break;
                case CERTAIN_DAYS_CONDITION:
                    CertainDaysCondition daysCondition;
                    if (selectedSchedule.condition.GetType() != typeof(CertainDaysCondition))
                        daysCondition = new CertainDaysCondition();
                    else
                        daysCondition = (CertainDaysCondition) selectedSchedule.condition;

                    daysCondition.sunday = sundayCheckBox.Checked;
                    daysCondition.monday = mondayCheckBox.Checked;
                    daysCondition.tuesday = tuesdayCheckBox.Checked;
                    daysCondition.wednesday = wednesdayCheckBox.Checked;
                    daysCondition.thursday = thursdayCheckBox.Checked;
                    daysCondition.friday = fridayCheckBox.Checked;
                    daysCondition.saturday = saturdayCheckBox.Checked;
                    selectedSchedule.condition = daysCondition;
                    break;
                case CERTAIN_MONTHS_CONDITION:
                    CertainMonthsCondition monthsCondition;
                    if (selectedSchedule.condition.GetType() != typeof(CertainMonthsCondition))
                        monthsCondition = new CertainMonthsCondition();
                    else
                        monthsCondition = (CertainMonthsCondition) selectedSchedule.condition;

                    monthsCondition.january = januaryCheckBox.Checked;
                    monthsCondition.february = februaryCheckBox.Checked;
                    monthsCondition.march = marchCheckBox.Checked;
                    monthsCondition.april = aprilCheckBox.Checked;
                    monthsCondition.may = mayCheckBox.Checked;
                    monthsCondition.june = juneCheckBox.Checked;
                    monthsCondition.july = julyCheckBox.Checked;
                    monthsCondition.august = augustCheckBox.Checked;
                    monthsCondition.september = septemberCheckBox.Checked;
                    monthsCondition.october = octoberCheckBox.Checked;
                    monthsCondition.november = novemberCheckBox.Checked;
                    monthsCondition.december = decemberCheckBox.Checked;
                    selectedSchedule.condition = monthsCondition;
                    break;
            }
        }

        private void andBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.DefaultExt = "exf";
            Dialog.Filter = "Exact Ferret Settings Files|*.exf";
            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = Dialog.FileName;
                SettingsSchedule schedule = new SettingsSchedule();
                schedule.and = true;
                schedule.file = new SettingsFile(new FileInfo(filePath));
                schedule.condition = new CertainTimesCondition();
                ((CertainTimesCondition)schedule.condition).stopTime = DateTime.Today.AddDays(1).AddSeconds(-1).TimeOfDay;
                schedules.Add(schedule);
                updateForm(true, schedule);
            }
        }

        private void orBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.DefaultExt = "exf";
            Dialog.Filter = "Exact Ferret Settings Files|*.exf";
            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = Dialog.FileName;
                SettingsSchedule schedule = new SettingsSchedule();
                schedule.and = false;
                schedule.file = new SettingsFile(new FileInfo(filePath));
                schedule.condition = new CertainTimesCondition();
                ((CertainTimesCondition) schedule.condition).stopTime = DateTime.Today.AddDays(1).AddSeconds(-1).TimeOfDay;
                schedules.Add(schedule);
                updateForm(false, schedule);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // Save the scheules
            PropertiesManager.setSettingsSchedules(schedules);
            DialogResult = DialogResult.OK;
        }

        private void conditionParameter_ValueChanged(object sender, EventArgs e)
        {
            if (!lockForm)
                updateCondition();
        }

        private void andBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lockForm)
                return;

            selectedSchedule = (SettingsSchedule) andBox.SelectedItem;
            lockForm = true;
            orBox.SelectedIndex = -1;
            lockForm = false;
            updateFormCondition();

            orRemoveButton.Enabled = false;
            if (andBox.SelectedIndex != -1)
                andRemoveButton.Enabled = true;
        }

        private void orBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lockForm)
                return;

            selectedSchedule = (SettingsSchedule) orBox.SelectedItem;
            lockForm = true;
            andBox.SelectedIndex = -1;
            lockForm = false;
            updateFormCondition();

            andRemoveButton.Enabled = false;
            if (orBox.SelectedIndex != -1)
                orRemoveButton.Enabled = true;
        }

        private void updateFormCondition()
        {
            lockForm = true;

            if (selectedSchedule != null)
            {
                conditionBox.Enabled = true;

                Condition condition = selectedSchedule.condition;

                if (condition.GetType() == typeof(CertainTimesCondition))
                {
                    conditionBox.SelectedIndex = 0;

                    CertainTimesCondition timesCondition = (CertainTimesCondition)condition;
                    certainTimesStartPicker.Value = DateTime.Today.Add(timesCondition.startTime);
                    certainTimesStopPicker.Value = DateTime.Today.Add(timesCondition.stopTime);
                }
                else if (condition.GetType() == typeof(CertainDaysCondition))
                {
                    conditionBox.SelectedIndex = 1;

                    CertainDaysCondition daysCondition = (CertainDaysCondition)condition;
                    sundayCheckBox.Checked = daysCondition.sunday;
                    mondayCheckBox.Checked = daysCondition.monday;
                    tuesdayCheckBox.Checked = daysCondition.tuesday;
                    wednesdayCheckBox.Checked = daysCondition.wednesday;
                    thursdayCheckBox.Checked = daysCondition.thursday;
                    fridayCheckBox.Checked = daysCondition.friday;
                    saturdayCheckBox.Checked = daysCondition.saturday;
                }
                else if (condition.GetType() == typeof(CertainMonthsCondition))
                {
                    conditionBox.SelectedIndex = 2;

                    CertainMonthsCondition monthsCondition = (CertainMonthsCondition)condition;
                    januaryCheckBox.Checked = monthsCondition.january;
                    februaryCheckBox.Checked = monthsCondition.february;
                    marchCheckBox.Checked = monthsCondition.march;
                    aprilCheckBox.Checked = monthsCondition.april;
                    mayCheckBox.Checked = monthsCondition.may;
                    juneCheckBox.Checked = monthsCondition.june;
                    julyCheckBox.Checked = monthsCondition.july;
                    augustCheckBox.Checked = monthsCondition.august;
                    septemberCheckBox.Checked = monthsCondition.september;
                    octoberCheckBox.Checked = monthsCondition.october;
                    novemberCheckBox.Checked = monthsCondition.november;
                    decemberCheckBox.Checked = monthsCondition.december;
                }
            }

            lockForm = false;
        }

        private void andRemoveButton_Click(object sender, EventArgs e)
        {
            if (selectedSchedule != null)
            {
                schedules.Remove(selectedSchedule);
                if (andBox.SelectedIndex > 0)
                    updateForm(true, (SettingsSchedule) andBox.Items[andBox.SelectedIndex - 1]);
                else if (andBox.Items.Count > 1)
                    updateForm(true, (SettingsSchedule)andBox.Items[1]);
                else
                    updateForm(true, null);
            }
        }

        private void orRemoveButton_Click(object sender, EventArgs e)
        {
            if (selectedSchedule != null)
            {
                schedules.Remove(selectedSchedule);
                if (orBox.SelectedIndex > 0)
                    updateForm(false, (SettingsSchedule)orBox.Items[orBox.SelectedIndex - 1]);
                else if (orBox.Items.Count > 1)
                    updateForm(false, (SettingsSchedule)orBox.Items[1]);
                else
                    updateForm(false, null);
            }
        }
    }
}
