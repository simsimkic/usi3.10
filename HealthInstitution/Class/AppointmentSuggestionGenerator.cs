using HealthInstitution.Repository;
using System;
using System.Collections.Generic;

namespace HealthInstitution.Class
{
    public class AppointmentSuggestionGenerator
    {

        private AppointmentScheduleRequest Request;

        public List<Appointment> Generate(AppointmentScheduleRequest request)
        {
            Request = request;
            List<Appointment> result = new();
            var suggestion = SuggestOnTerms();
            if (suggestion == null)
            {
                if (request.PrioritySuggestion == Priority.DOCTOR)
                    suggestion = SuggestWithDoctorPriority();
                else
                    suggestion = SuggestWithTimePriority();

                if (suggestion == null)
                    return SuggestWithoutPriority();
            }
            result.Add(suggestion);
            return result;
        }


        private List<Appointment> SuggestAllOnTerms()
        {
            var suggestions = new List<Appointment>();
            DateTime start = Request.Range.Start.ToDateTime(Request.Range.TimeStart);
            while (true)
            {
                var timeslotOption = Request.Range.GenerateNextOption(start);
                if (timeslotOption == null) break;

                var selectedRoom = GlobalRepository.schedule.CheckAvailability(timeslotOption, Request.DoctorUsername,
                    Request.PatientUsername, RoomType.ExaminationRoom);

                if (selectedRoom != null)
                {
                    suggestions.Add(new Appointment(Request.DoctorUsername, Request.PatientUsername,
                       timeslotOption, AppointmentType.EXAMINATION, (int)selectedRoom));
                }
                start = timeslotOption.Start.Add(Timeslot.ExaminationDuration.TimeSpan);
            }
            return suggestions;
        }


        private Appointment? SuggestOnTerms()
        {
            DateTime start = Request.Range.Start.ToDateTime(Request.Range.TimeStart);
            while (true)
            {
                var timeslotOption = Request.Range.GenerateNextOption(start);
                if (timeslotOption == null) break;

                var selectedRoom = GlobalRepository.schedule.CheckAvailability(timeslotOption, Request.DoctorUsername,
                    Request.PatientUsername, RoomType.ExaminationRoom);

                if (selectedRoom != null)
                {
                    return new Appointment(Request.DoctorUsername, Request.PatientUsername,
                        timeslotOption, AppointmentType.EXAMINATION, (int)selectedRoom);
                }
                start = timeslotOption.Start.Add(Timeslot.ExaminationDuration.TimeSpan);
            }
            return null;
        }


        private Appointment? SuggestWithDoctorPriority()
        {
            Request.Range.Set24Hours();
            Appointment? suggestion = SuggestOnTerms();
            if (suggestion != null) return suggestion;

            TimeRange initialRange = Request.Range;
            for (int i = 1; i < 30; i++)
            {
                Request.Range = initialRange.GetRangeAfterDays(i);
                suggestion = SuggestOnTerms();
                if (suggestion != null) break;
            }
            return suggestion;
        }


        private List<Appointment> SuggestWithoutPriority()
        {
            List<Appointment> result = new();
            Request.Range.Set24Hours();
            TimeRange initialRange = Request.Range;
            int i = 0;
            while (result.Count < 3)
            {
                foreach (Doctor doctor in DoctorRepository.Doctors)
                {
                    Request.DoctorUsername = doctor.Username;
                    if (i != 0)
                        Request.Range = initialRange.GetRangeAfterDays(i);
                    result.AddRange(SuggestAllOnTerms());
                    if (result.Count >= 3) break;
                }
                i++;
            }
            return result;
        }


        private Appointment? SuggestWithTimePriority()
        {
            Appointment? suggestion = null;
            string initialDoctor = Request.DoctorUsername;
            foreach (Doctor doctor in DoctorRepository.Doctors)
            {
                if (doctor.Username == initialDoctor) continue;

                Request.DoctorUsername = doctor.Username;
                suggestion = SuggestOnTerms();
                if (suggestion != null) break;
            }
            return suggestion;
        }
    }
}