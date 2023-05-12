using HealthInstitution.ActionLog;
using HealthInstitution.Class;
using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HealthInstitution.PatientView
{
    public class AppointmentSuggestionService
    {
        public AppointmentSuggestionService() { }

        public List<Appointment> GenerateSuggestion(AppointmentScheduleRequest request)
        {
            AppointmentSuggestionGenerator generator = new();
            return generator.Generate(request);
        }

        public bool AddAppointment(Appointment appointment)
        {
            return GlobalRepository.schedule.AddAppointment(appointment);
        }

        public bool PotentiallyBlock()
        {
            string patientUsername = GlobalRepository.currentUser.Username;
            Log.AddItem(new(patientUsername, ActionLog.Action.CREATE));
            bool readyForBlocking = Log.CheckCreateRequests(patientUsername);
            if (readyForBlocking)
            {
                PatientRepository.Block((Patient)GlobalRepository.currentUser);
            }
            return readyForBlocking;
        }
    }
}
