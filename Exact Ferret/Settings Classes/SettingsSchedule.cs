using Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exact_Ferret.Settings_Classes
{
    class SettingsFile
    {
        public FileInfo info { get; set; }
        public SettingsFile(FileInfo info)
        {
            this.info = info;
        }
        public override string ToString()
        {
            return info.Name;
        }
    }

    class Condition
    {

    }

    class CertainTimesCondition : Condition
    {
        public TimeSpan startTime { get; set; }
        public TimeSpan stopTime { get; set; }
        public override string ToString()
        {
            return "times," + startTime.Ticks + "," + stopTime.Ticks;
        }
    }

    class CertainDaysCondition : Condition
    {
        public bool sunday { get; set; }
        public bool monday { get; set; }
        public bool tuesday { get; set; }
        public bool wednesday { get; set; }
        public bool thursday { get; set; }
        public bool friday { get; set; }
        public bool saturday { get; set; }
        public override string ToString()
        {
            return "days," + sunday + "," + monday + "," + tuesday + "," + wednesday + "," + thursday + "," + friday + "," + saturday;
        }
    }

    class CertainMonthsCondition : Condition
    {
        public bool january { get; set; }
        public bool february { get; set; }
        public bool march { get; set; }
        public bool april { get; set; }
        public bool may { get; set; }
        public bool june { get; set; }
        public bool july { get; set; }
        public bool august { get; set; }
        public bool september { get; set; }
        public bool october { get; set; }
        public bool november { get; set; }
        public bool december { get; set; }
        public override string ToString()
        {
            return "months," + january + "," + february + "," + march + "," + april + "," + may + "," + june + "," +
                    july + "," + august + "," + september + "," + october + "," + november + "," + december;
        }
    }

    class SettingsSchedule
    {
        public SettingsFile file { get; set; }
        public Condition condition { get; set; }
        public bool and { get; set; }
        public bool or
        {
            get
            {
                return !and;
            }
        }
        public override string ToString()
        {
            return file.ToString();
        }

        public static SettingsSchedule[] getScheduledSettings()
        {
            List<SettingsSchedule> schedules = PropertiesManager.getSettingsSchedules();
            List<SettingsSchedule> applicableSchedules = new List<SettingsSchedule>();

            // Sort the schedules by file name into a table
            Hashtable schedulesByFile = new Hashtable();
            foreach (SettingsSchedule schedule in schedules)
            {
                string fileName = schedule.file.info.FullName;
                List<SettingsSchedule> schedulesForFile = (List<SettingsSchedule>)schedulesByFile[fileName];
                if (schedulesForFile == null)
                {
                    schedulesForFile = new List<SettingsSchedule>();
                    schedulesByFile.Add(fileName, schedulesForFile);
                }

                schedulesForFile.Add(schedule);
            }

            // Check each file list to see if it applies
            foreach (string fileName in schedulesByFile.Keys)
            {
                bool conditionsMet = true;
                int andConditionsFound = 0;

                List<SettingsSchedule> schedulesForFile = (List<SettingsSchedule>)schedulesByFile[fileName];
                foreach (SettingsSchedule schedule in schedulesForFile)
                {
                    bool conditionApplies = false;
                    if (schedule.condition.GetType() == typeof(CertainTimesCondition))
                    {
                        CertainTimesCondition timesCondition = (CertainTimesCondition)schedule.condition;
                        TimeSpan currentTime = DateTime.Now.TimeOfDay;
                        conditionApplies = currentTime <= timesCondition.stopTime && currentTime >= timesCondition.startTime;
                    }
                    else if (schedule.condition.GetType() == typeof(CertainDaysCondition))
                    {
                        CertainDaysCondition daysCondition = (CertainDaysCondition)schedule.condition;
                        switch (DateTime.Today.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                conditionApplies = daysCondition.sunday;
                                break;
                            case DayOfWeek.Monday:
                                conditionApplies = daysCondition.monday;
                                break;
                            case DayOfWeek.Tuesday:
                                conditionApplies = daysCondition.tuesday;
                                break;
                            case DayOfWeek.Wednesday:
                                conditionApplies = daysCondition.wednesday;
                                break;
                            case DayOfWeek.Thursday:
                                conditionApplies = daysCondition.thursday;
                                break;
                            case DayOfWeek.Friday:
                                conditionApplies = daysCondition.friday;
                                break;
                            case DayOfWeek.Saturday:
                                conditionApplies = daysCondition.saturday;
                                break;
                        }
                    }
                    else if (schedule.condition.GetType() == typeof(CertainMonthsCondition))
                    {
                        CertainMonthsCondition monthsCondition = (CertainMonthsCondition)schedule.condition;
                        switch (DateTime.Today.Month)
                        {
                            case 1:
                                conditionApplies = monthsCondition.january;
                                break;
                            case 2:
                                conditionApplies = monthsCondition.february;
                                break;
                            case 3:
                                conditionApplies = monthsCondition.march;
                                break;
                            case 4:
                                conditionApplies = monthsCondition.april;
                                break;
                            case 5:
                                conditionApplies = monthsCondition.may;
                                break;
                            case 6:
                                conditionApplies = monthsCondition.june;
                                break;
                            case 7:
                                conditionApplies = monthsCondition.july;
                                break;
                            case 8:
                                conditionApplies = monthsCondition.august;
                                break;
                            case 9:
                                conditionApplies = monthsCondition.september;
                                break;
                            case 10:
                                conditionApplies = monthsCondition.october;
                                break;
                            case 11:
                                conditionApplies = monthsCondition.november;
                                break;
                            case 12:
                                conditionApplies = monthsCondition.december;
                                break;
                        }
                    }
                    else
                        continue;

                    if (schedule.or)
                    {
                        if (conditionApplies)
                        {
                            conditionsMet = true;
                            applicableSchedules.Add(schedule);
                            break;
                        }
                    }
                    else
                    {
                        conditionsMet &= conditionApplies;
                        andConditionsFound++;
                    }
                }

                if (conditionsMet && andConditionsFound > 0)
                {
                    applicableSchedules.Add(schedulesForFile[0]);
                }
            }

            return applicableSchedules.ToArray();
        }
    }
}
