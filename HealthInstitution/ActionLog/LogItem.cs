using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.ActionLog
{

    public enum Action
    {
        CREATE = 0,
        EDIT = 1,
        DELETE = 2
    }

    public class LogItem
    {
        public string PatientUsername { get; set; }

        public Action RecordedAction { get; set; }

        public DateTime TimeOf { get; set; }


        [JsonConstructor]
        public LogItem(string patientUsername, Action recordedAction, DateTime timeOf)
        {
            PatientUsername = patientUsername;
            RecordedAction = recordedAction;
            TimeOf = timeOf;
        }

        public LogItem(string patientUsername, Action recordedAction)
        {
            PatientUsername = patientUsername;
            RecordedAction = recordedAction;
            TimeOf = DateTime.Now;
        }
    }
}
