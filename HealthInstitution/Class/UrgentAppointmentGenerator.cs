using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HealthInstitution.Class
{
    public class UrgentAppointmentGenerator
    {
        public class AppointmentWithTime
        {
            public DateTime Time { get; set; }
            public int Id { get; set; }

            public AppointmentWithTime(DateTime time, int id)
            {
                Time = time;
                Id = id;
            }
        }

        public List<AppointmentWithTime> SortedAppointments { get; set; }

        public UrgentAppointmentGenerator() { SortedAppointments = new List<AppointmentWithTime>(); }

        public Appointment FindUrgentAppointment(Patient patient,
            AppointmentType type, string specialization, int durationMinutes = 15)
        {
            Duration duration = new Duration(TimeSpan.FromMinutes(durationMinutes));

            DateTime start = DateTime.Now.AddMinutes(15 - (DateTime.Now.Minute % 15));
            DateTime end = start.AddHours(2);

            while (true)
            {
                var timeslot = new Timeslot(start, duration);
                var availableDoctors = GlobalRepository.schedule.FindAvailableDoctors(timeslot, specialization);
                if (availableDoctors.Count > 0)
                {
                    return new Appointment(availableDoctors[0].Username, patient.Username, timeslot, type);
                }
                start = start.AddMinutes(15);
                if (start >= end) break;
            }

            return null;
        }

        public Appointment FindFirstNextAppointment(Appointment appointment)
        {
            DateTime start = DateTime.Now.AddMinutes(15 - (DateTime.Now.Minute % 15));
            DateTime end = start.AddDays(30);
            RoomType roomType = Appointment.AppointmentToRoomType(appointment.Type);
            while (true)
            {
                Timeslot timeslot = new Timeslot(start, appointment.TimeOf.DurationOf);
                int? roomNumber = GlobalRepository.schedule.CheckAvailability(timeslot, appointment.DoctorUsername, appointment.PatientUsername, roomType);
                if (roomNumber != null)
                {
                    return new Appointment(appointment.DoctorUsername, appointment.PatientUsername,
                        timeslot, appointment.Type, (int)roomNumber);
                }
                start = start.AddMinutes(15);
                if (start >= end) break;
            }
            return null;
        }

        public List<Appointment> FindAppointmentsToPostpone()
        {
            List<Appointment> candidates = GlobalRepository.schedule.FindAllApointments(new Timeslot(DateTime.Now, new Duration(new(2, 0, 0))));
            foreach (Appointment candidate in candidates)
            {
                DateTime nextAppointmentTime = FindFirstNextAppointment(candidate).TimeOf.Start;
                AddIntoSortedAppointments(new AppointmentWithTime(nextAppointmentTime, candidate.Id));
            }

            return GenerateTopFiveAppointments();
            
        }

        public void AddIntoSortedAppointments(AppointmentWithTime newAppointment)
        {
            if(SortedAppointments.Count == 0)
            {
                SortedAppointments.Add(newAppointment);
                return;
            }

            int i;
            for (i = 0; i < SortedAppointments.Count; ++i)
            {
                if (i == SortedAppointments.Count - 1)
                    break;

                if ( (newAppointment.Time >= SortedAppointments[i].Time) 
                    && (newAppointment.Time < SortedAppointments[i+1].Time) )
                    break;
            }

            SortedAppointments.Insert(i, newAppointment);
        }

        public List<Appointment> GenerateTopFiveAppointments()
        {
            int i = 0;

            List<Appointment> topFive = new List<Appointment>();
            foreach (var a in SortedAppointments)
            {
                topFive.Add(GlobalRepository.schedule.FindById(a.Id));
                i++;

                if (i == 5) break;
            }
                
            return topFive;
        }
    }
}
