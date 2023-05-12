using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HealthInstitution.Class
{
    public class Patient : User
    {
        public MedicalRecord MedicalRecord { get; set; } = new MedicalRecord();
        public bool Blocked { get; set; }


        public Patient(string name, string surname, string username,
            string password, bool blocked, MedicalRecord medicalRecord)

        {
            this.Name = name;
            this.Surname = surname;
            this.Username = username;
            this.Password = password;
            this.MedicalRecord = medicalRecord;
            this.Blocked = blocked;
        }

        public Patient()
        {
            this.Name = "";
            this.Surname = "";
            this.Username = "";
            this.Password = "";
            this.Blocked = false;
            this.MedicalRecord = new MedicalRecord();
        }

        public void Update(Patient newPatient)
        {
            Name = newPatient.Name;
            Surname = newPatient.Surname; 
            Username = newPatient.Username;
            Password = newPatient.Password;
            MedicalRecord = newPatient.MedicalRecord;
            Blocked = newPatient.Blocked;
        }

    }
}
