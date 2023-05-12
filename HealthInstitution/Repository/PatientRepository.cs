using HealthInstitution.Class;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Repository
{
    internal class PatientRepository
    {
        private static readonly string _filenamePatients = "../../../Data/Users/patients.json";

        public static List<Patient> Patients { get; set; } = new List<Patient>();

        static PatientRepository()
        {
            if (File.Exists(_filenamePatients))
            {
                var patients = (List<Patient>?)FileLoader.Deserialize<Patient>(_filenamePatients);

                if (patients != null)
                {
                    Patients = patients;
                }

            }
        }

        public static void Save()
        {
            FileLoader.Serialize<Patient>(Patients, _filenamePatients);
        }

        public static Patient? FindPatient(string username)
        {
            return Patients.Find(p => p.Username == username);
        }

        public static bool DeletePatient(string username)
        {
            int deleted = Patients.RemoveAll(p => p.Username == username);
            if (deleted > 0)
            {
                Save();
                return true;
            }

            return false;
        }

        public static void AddPatient(Patient newPatient)
        {
            Patients.Add(newPatient);
            Save();
        }

        public static bool UpdatePatient(Patient oldPatient, Patient newPatient)
        {
            foreach (var patient in Patients)
            {
                if (oldPatient.Username == patient.Username)
                {
                    patient.Update(newPatient);
                    Save();
                    return true;
                }
            }

            return false;
        }

        public static bool UpdateMedicalRecord(List<string> diseases, List<string> allergies, string username)
        {
            foreach(Patient patient in Patients)
            {
                if(patient.Username == username)
                {
                    patient.MedicalRecord.UpdateLists(diseases, allergies);
                    Save();
                    return true;
                }
            }
            return false;

        }

        public static Patient? PatientExists(string username, string password)
        {
            foreach (Patient patient in PatientRepository.Patients)
            {
                if (username == patient.Username && password == patient.Password)
                {
                    return patient;
                }
            }
            return null;
        }

        public static void Block(Patient patient)
        {
            patient.Blocked = true;
            Save();
        }

        public static bool IsUniqueUsername(string username) {
            return FindPatient(username) == null;
        }

        public static bool IsValidName(string name)
        {
            return name.Length > 2 && name.All(char.IsLetter);
        }

        public static bool IsValidSurname(string surname)
        {
            return surname.Length > 5 && surname.All(char.IsLetter);
        }

        public static bool IsValidPassword(string name)
        {
            return name.Length > 8;
        }

        public static bool IsValidHeight(string height)
        {
            return height.All(char.IsDigit);
        }

        public static bool IsValidWeight(string height)
        {
            return height.All(char.IsDigit);
        }

        public static List<string> PatientsToStrings()
        {
            List<string> patientsString = new List<string>();
            foreach (Patient patient in Patients)
            {
                patientsString.Add(patient.Name + " " + patient.Surname + " " + patient.Username);
            }
            return patientsString;
        }


    }
}
