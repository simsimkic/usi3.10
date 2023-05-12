using HealthInstitution.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Repository
{
    public static class DoctorRepository
    {
        public static List<Doctor> Doctors { get; set; } = new List<Doctor>();

        static DoctorRepository()
        {
            string _filenameDoctors = "../../../Data/Users/doctors.json";

            if (File.Exists(_filenameDoctors))
            {
                var doctors = (List<Doctor>?)FileLoader.Deserialize<Doctor>(_filenameDoctors);

                if (doctors != null)
                {
                    Doctors = doctors;
                }

            }
        }

        public static Doctor? FindDoctor(string username)
        {
            foreach (var doctor in Doctors)
            {
                if (doctor.Username == username)
                    return doctor;
            }
            return null;
        }
    

        public static Doctor? DoctorExists(string username, string password)
        {
            foreach (Doctor doctor in Doctors)
            {
                if (username == doctor.Username && password == doctor.Password)
                {
                    return doctor;
                }
            }
            return null;
        }

        public static List<string> GetAllSpecializations()
        {
            SortedSet<string> specializations = new();

            foreach(var doctor in Doctors)
                specializations.Add(doctor.Specialization);

            return specializations.ToList();
        }

    }
}
