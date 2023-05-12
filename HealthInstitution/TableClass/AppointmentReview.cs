using HealthInstitution.Class;
using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.TableClass
{
    public class AppointmentReview
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DoctorSpecialization { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }

        public AppointmentStatus Status { get; set; }

        public AppointmentType Type { get; set; }

        public Anamnesis Anamnesis { get; set; }

        public int RoomNumber { get; set; }

        public AppointmentReview(int id, string name, string surname, string doctorSpecialization,
            string date, string time, AppointmentStatus status, AppointmentType type, int roomNumber)
        {
            Id = id; 
            Name = name;
            Surname = surname;
            DoctorSpecialization = doctorSpecialization;
            StartDate = date;
            StartTime = time;
            Status = status;
            Type = type;
            Anamnesis = new Anamnesis();
            RoomNumber = roomNumber;
        }

        public AppointmentReview(int id, string name, string surname, string doctorSpecialization,
            string date, string time, AppointmentStatus status, AppointmentType type, Anamnesis anamnesis, int roomNumber)
        {
            Id = id;
            Name = name;
            Surname = surname;
            DoctorSpecialization = doctorSpecialization;
            StartDate = date;
            StartTime = time;
            Status = status;
            Type = type;
            Anamnesis = anamnesis;
            RoomNumber = roomNumber;
        }

        public Appointment ToAppointment()
        {
            return GlobalRepository.schedule.FindById(Id);
        }
    }
}
