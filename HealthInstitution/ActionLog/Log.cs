using HealthInstitution.Repository;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.ActionLog
{
    public static class Log
    {
        private readonly static string _filename = "../../../Data/Schedule/log.json";

        public static List<LogItem> Items { get; set; }

        static Log()
        {
            Items = (List<LogItem>)FileLoader.Deserialize<LogItem>(_filename);
        }

        public static void Save()
        {
            FileLoader.Serialize<LogItem>(Items, _filename);
        }

        public static void AddItem(LogItem item)
        {
            Items.Add(item);
            Save();
        }


        //In the last 30 days
        public static List<LogItem> GetAllActionsFor(string patientUsername)
        {
            return Items.Where(item => 
                item.PatientUsername == patientUsername
                && item.TimeOf.AddDays(30) >= DateTime.Now.Date).ToList();
        }


        public static bool CheckCreateRequests(string patientUsername)
        {
            var items = GetAllActionsFor(patientUsername);
            int counter = items.Where(item => item.RecordedAction == Action.CREATE).Count();
            if (counter < 8) return false;
            return true;

        }

        public static bool CheckEditDeleteRequests(string patientUsername)
        {
            var items = GetAllActionsFor(patientUsername);
            int counter = items.Where(item => item.RecordedAction == Action.EDIT
                            || item.RecordedAction == Action.DELETE).Count();
            if (counter < 5) return false;
            return true;
        }
    }
}
