using HealthInstitution.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Repository
{
    public static class NurseRepository
    {
        public static List<Nurse> Nurses { get; set; } = new List<Nurse>();

        static NurseRepository()
        {
            string _filenameNurses = "../../../Data/Users/nurses.json";

            if (File.Exists(_filenameNurses))
            {
                var nurses = (List<Nurse>?)FileLoader.Deserialize<Nurse>(_filenameNurses);

                if (nurses != null)
                {
                    Nurses = nurses;
                }

            }
        }

        public static Nurse? NurseExists(string username, string password)
        {
            foreach (Nurse nurse in NurseRepository.Nurses)
            {
                if (username == nurse.Username && password == nurse.Password)
                {
                    return nurse;
                }
            }
            return null;
        }
    }
}
