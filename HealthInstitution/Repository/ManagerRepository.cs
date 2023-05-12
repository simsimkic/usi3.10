using HealthInstitution.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Repository
{
    public static class ManagerRepository
    {
        public static List<Manager> Managers { get; set; } = new List<Manager>();

        static ManagerRepository()
        {
            string _filenameManagers = "../../../Data/Users/managers.json";

            if (File.Exists(_filenameManagers))
            {
                var managers = (List<Manager>?)FileLoader.Deserialize<Manager>(_filenameManagers);
                if (managers != null) Managers = managers;
            }
        }

        public static Manager? ManagerExists(string username, string password)
        {
            foreach (Manager manager in Managers)
            {
                if (username == manager.Username && password == manager.Password) return manager;
            }
            return null;
        }
    }
}
