using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Class
{
    public enum Priority
    {
        DOCTOR,
        TIME
    }
    public class AppointmentScheduleRequest
    {

        public string PatientUsername { get; set; }
        public string DoctorUsername { get; set; }
        public TimeRange Range { get; set; }
        public Priority PrioritySuggestion {  get; set; }

        public AppointmentScheduleRequest(string patientUsername, string doctorUsername, TimeRange time, Priority priority) { 
            PatientUsername = patientUsername;
            DoctorUsername = doctorUsername;
            Range = time;
            PrioritySuggestion = priority;
        }

    }
}
