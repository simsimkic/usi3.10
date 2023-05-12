using HealthInstitution.Repository;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HealthInstitution.Class
{
    

    class Schedule
    {
        public static Duration ExaminationDuration = new Duration(new(0, 15, 0));

        private readonly string _filenameSchedule = "../../../Data/Schedule/schedule.json";

        public List<Appointment> Appointments { get; set; }


        public Schedule() {
            Appointments = (List<Appointment>)FileLoader.Deserialize<Appointment>(_filenameSchedule);
        }

        public Appointment FindById(int id)
        {
            return Appointments.Where(a => a.Id == id).ToList()[0];
        }

        public void Save()
        {
            FileLoader.Serialize<Appointment>(Appointments, _filenameSchedule);
        }

        public int CreateNewId()
        {
            var ids = Appointments.Select(a => a.Id).ToList();
            if (ids.Count == 0)
                return 1;

            return ids.Max() + 1;
        }

        public bool AddAppointment(Appointment appointment) {
            RoomType roomType = Appointment.AppointmentToRoomType(appointment.Type);
            int? selectedRoom = CheckAvailability(appointment.TimeOf, appointment.DoctorUsername,
                appointment.PatientUsername, roomType);
            if (selectedRoom == null) return false;
            appointment.RoomNumber = (int)selectedRoom;
            Appointments.Add(appointment);
            Save();
            return true;
        }

        public bool ValidPatientForReview(string patientUsername, string doctorUsername)
        {
            foreach (Appointment appointment in Appointments)
            {
                if (appointment.Status == AppointmentStatus.FINISHED
                    && appointment.DoctorUsername == doctorUsername
                    && appointment.PatientUsername == patientUsername)
                {
                    return true;
                }
            }
            return false;

        }

        public int? CheckAvailability(Timeslot timeslot, string doctorUsername, string patientUsername, RoomType type, int id = 0)
        {
            if (timeslot.Start <  DateTime.Now) return null;
            if (!(IsDoctorAvailableFor(doctorUsername, timeslot, id)
                && IsPatientAvailableFor(patientUsername, timeslot, id)))
                return null;
            return GetAvailableRoom(type, timeslot, id);
        }

        public void CancelAppointment(Appointment appointment)
        {
            appointment.Cancel();
            Save();
        }

        public void UpdateAppointmenAnamnesis(Appointment appointment,
            List<string> newSymptoms, List<string> newDiseases, List<string> newAllergies, string report)
        {
            appointment.UpdateAnamnesis(newSymptoms, newDiseases, newAllergies, report);

            Save();
        }

        public bool UpdateAppointment(Appointment appointment, Timeslot newTimeslot, string newDoctorUsername)
        {
            var roomType = Appointment.AppointmentToRoomType(appointment.Type);
            var newRoom = CheckAvailability(newTimeslot, newDoctorUsername, appointment.PatientUsername, roomType, appointment.Id);
            if (newRoom == null) return false;
            appointment.Update(newTimeslot, newDoctorUsername, (int)newRoom);
            Save();
            return true;
        }

        public bool UpdateAppointmentForDoctor(Appointment appointment, Timeslot newTimeslot, string newPatientUsername, AppointmentType newType)
        {
            var roomType = Appointment.AppointmentToRoomType(newType);
            var newRoom = CheckAvailability(newTimeslot, appointment.DoctorUsername, newPatientUsername, roomType, appointment.Id);
            if (newRoom == null) return false;
            appointment.UpdateForDoctor(newTimeslot, newPatientUsername, newType, (int)newRoom);
            Save();
            return true;
        }

        public void UpdateStatus(Appointment appointment, AppointmentStatus newStatus)
        {
            appointment.UpdateStatus(newStatus);
            Save();
        }
        private List<Appointment> GetAppointmentsIn(int roomNumber)
        {
            var filtered = new List<Appointment>();
            foreach (var appointment in Appointments)
                if (appointment.RoomNumber == roomNumber)
                    filtered.Add(appointment);
            return filtered;
        }

        private bool IsRoomAvailable(int roomNumber, Timeslot timeslot, int id = 0)
        {
            var appointmentsInRoom = GetAppointmentsIn(roomNumber);
            foreach (var appointment in appointmentsInRoom)
            {
                if (appointment.Id == id) continue;
                if (appointment.TimeOf.OverlapsWith(timeslot))
                    return false;
            }
            return true;
        }

        private int? GetAvailableRoom(RoomType type, Timeslot timeslot, int id=0)
        {
            
            var roomsOfType = GlobalRepository.equipmentRoomRepository.GetRoomsOfType(type);
            foreach (var room in roomsOfType) { 
                if (IsRoomAvailable(room.Number, timeslot, id))
                        return room.Number;
            }
            return null;
        }

        public List<Appointment> FindForDoctor(string doctorUsername)
        {
            var appointments = new List<Appointment>();

            foreach(var appointment in Appointments)
                if (appointment.DoctorUsername == doctorUsername)
                    appointments.Add(appointment);
            
            return appointments;
        }

        public List<Appointment> FindActiveForDoctor(string doctorUsername)
        {
            var appointments = new List<Appointment>();
            var allAppointments = FindForDoctor(doctorUsername);
            foreach (var appointment in allAppointments)
                if (appointment.Status == AppointmentStatus.ACTIVE)
                    appointments.Add(appointment);
            return appointments;
        }

        public List<Appointment> FindForPatient(string patientUsername)
        {
            var appointments = new List<Appointment>();

            foreach (var appointment in Appointments)
                if (appointment.PatientUsername == patientUsername)
                    appointments.Add(appointment);
            return appointments;
        }

        public List<Appointment> FindActiveForPatient(string patientUsername)
        {
            var appointments = new List<Appointment>();
            var allAppointments = FindForPatient(patientUsername);
            foreach (var appointment in allAppointments)
                if (appointment.Status == AppointmentStatus.ACTIVE)
                    appointments.Add(appointment);
            return appointments;
        }


        public List<Doctor> FindAvailableDoctors(Timeslot timeslot)
        {
            return DoctorRepository.Doctors
                .Where(d => IsDoctorAvailableFor(d.Username, timeslot)).ToList();
        }

        public List<Doctor> FindAvailableDoctors(Timeslot timeslot, string specialization)
        {
            return DoctorRepository.Doctors
                .Where(d => (IsDoctorAvailableFor(d.Username, timeslot) && 
                d.Specialization == specialization)).ToList();
        }

        public List<Patient> FindAvailablePatients(Timeslot timeslot, int id = 0)
        {
            return PatientRepository.Patients
                .Where(d => IsPatientAvailableFor(d.Username, timeslot, id)).ToList();
        }


        public bool IsDoctorAvailableFor(string username, Timeslot timeslot, int id = 0)
        {
            var appointments = FindActiveForDoctor(username);
            foreach(Appointment appointment in appointments)
            {
                if (appointment.Id == id) continue;
                if (timeslot.OverlapsWith(appointment.TimeOf))
                    return false;
            }
            return true;
        }


        private bool IsPatientAvailableFor(string username, Timeslot timeslot, int id = 0)
        {
            var appointments = FindActiveForPatient(username);
            foreach (Appointment appointment in appointments)
            {
                if (appointment.Id == id) continue;
                if (timeslot.OverlapsWith(appointment.TimeOf))
                    return false;
            }
            return true;
        }

        public List<Appointment> FindUpcommingForPatient(string username)

        {
            var upcomming = new List<Appointment>();
            var allAppointments = FindForPatient(username);
            foreach (Appointment appointment in allAppointments) 
                if (appointment.TimeOf.Start > DateTime.Now.Date)
                    upcomming.Add(appointment);

            return upcomming;
        }

        public List<Appointment> FindUpcommingForDoctor(string username)
        {
            var appointments = new List<Appointment>();
            var allAppointments = FindForDoctor(username);
            foreach (Appointment appointment in allAppointments)
                if (appointment.TimeOf.Start > DateTime.Now.Date)
                    appointments.Add(appointment);
            
            return appointments;
        }

        public List<Appointment> FindAllApointments(Timeslot timeslot)
        {
            List<Appointment> appointments = new List<Appointment>();
            foreach(Appointment appointment in Appointments)
            {
                if (appointment.TimeOf.OverlapsWith(timeslot))
                    appointments.Add(appointment);
            }
            return appointments;
        }

        public List<Appointment> FindPastForPatient(string username)
        {
            var past = new List<Appointment>();
            var allAppointments = FindForPatient(username);
            foreach(Appointment appointment in allAppointments)
                if (appointment.TimeOf.Start < DateTime.Now)
                    past.Add(appointment);
            return past;
        }
    }
}
