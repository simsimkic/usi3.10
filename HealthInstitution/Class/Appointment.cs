using Accessibility;
using HealthInstitution.Repository;
using HealthInstitution.TableClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace HealthInstitution.Class
{

    public enum AppointmentType
    {
        EXAMINATION = 0,
        OPERATION = 1
    }

    public enum AppointmentStatus
    {
        ACTIVE = 0,
        CANCELED = 1,
        FINISHED = 2,
        WAITING_FOR_DOCTOR = 3
    }

    public class Appointment
    {
        public int Id {  get; }

        public Timeslot TimeOf { get; set; }

        public string DoctorUsername { get; set; }

        public string PatientUsername { get; set; }

        public AppointmentType Type { get; set; }

        public AppointmentStatus Status { get; set; }

        public Anamnesis Anamnesis { get; set; }

        public int RoomNumber { get; set; }


        [JsonConstructor]
        public Appointment(int id, Timeslot timeOf, string doctorUsername,
            string patientUsername, AppointmentType type, AppointmentStatus canceled,
            Anamnesis anamnesis, int roomNumber)
        {
            Id = id;
            TimeOf = timeOf;
            DoctorUsername = doctorUsername;
            PatientUsername = patientUsername;
            Type = type;
            Status = canceled;
            Anamnesis = anamnesis;
            RoomNumber = roomNumber;
        }

        public Appointment(string doctorUsername, string patientUsername, Timeslot timeOf, AppointmentType type, int roomNumber=0)
        {
            Id = GlobalRepository.schedule.CreateNewId();
            DoctorUsername = doctorUsername;
            PatientUsername = patientUsername;
            TimeOf = timeOf;
            Type = type;
            Status = AppointmentStatus.ACTIVE;
            Anamnesis = new Anamnesis();
            RoomNumber = roomNumber;
        }


        public bool ValidForUpdate()
        {
            return DateTime.Now.AddDays(1) < TimeOf.Start;
        }

        public bool ValidForDoctorReview(DateTime time)
        {
            return time <= TimeOf.Start && TimeOf.Start < time.AddDays(4);
        }

        public void Update(Timeslot newTimeslot, string newDoctorUsername, int newRoom)
        {
            TimeOf = newTimeslot;
            DoctorUsername = newDoctorUsername;
            RoomNumber = newRoom;
        }

        public void UpdateStatus(AppointmentStatus newStatus)
        {
            Status = newStatus;
        }


        public void UpdateForDoctor(Timeslot newTimeslot, string newPatientUsername, AppointmentType newtype, int newRoom)
        {
            TimeOf = newTimeslot;
            PatientUsername = newPatientUsername;
            Type = newtype;
            RoomNumber = newRoom;
        }

        public void UpdateAnamnesis(List<string> newSymptoms, List<string> newDiseases, List<string> newAllergies, string report)
        {
            Anamnesis.Update(newSymptoms, newDiseases, newAllergies, report);
        }

        public void Cancel()
        {
            Status = AppointmentStatus.CANCELED;
        }

        public static RoomType AppointmentToRoomType(AppointmentType appointmentType)
        {
            switch (appointmentType)
            {
                case AppointmentType.OPERATION: return RoomType.OperationRoom;
                case AppointmentType.EXAMINATION: return RoomType.ExaminationRoom;
                default: throw new Exception("Conversion between Appointment and Room types is only possible for operations and examinations.\n");
            }
        }

        public AppointmentReview ToReview()
        {
            var doctor = DoctorRepository.FindDoctor(DoctorUsername);
            string name = "";
            string surname = "";
            string specialization = "";
            if (doctor != null)
            {
                name = doctor.Name;
                surname = doctor.Surname;
                specialization = doctor.Specialization;
            }
            return new AppointmentReview(Id, name, surname, specialization,
                TimeOf.Start.ToShortDateString(), TimeOf.Start.ToShortTimeString(), Status, Type, Anamnesis, RoomNumber);
        }

        public AppointmentReview? ToReviewDoctor(DateTime time)
        {
            if (!ValidForDoctorReview(time)) return null;

            var patient = PatientRepository.FindPatient(PatientUsername);
            string name = "";
            string surname = "";
            if (patient != null)
            {
                name = patient.Name;
                surname = patient.Surname;
            }
            return new AppointmentReview(Id, name, surname, "",
                TimeOf.Start.ToShortDateString(), TimeOf.Start.ToShortTimeString(), Status, Type, RoomNumber);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append("Date: " + TimeOf.Start.ToShortDateString());
            stringBuilder.Append(" Time: " + TimeOf.Start.ToShortTimeString());
            Doctor doctor = DoctorRepository.FindDoctor(DoctorUsername);
            stringBuilder.Append(" Doctor: " + doctor);
            return stringBuilder.ToString();
        }
    }
}
