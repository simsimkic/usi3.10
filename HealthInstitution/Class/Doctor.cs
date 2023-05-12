using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Class
{
    public class Doctor : User
    {
        public string Specialization { get; set; }

        public override string ToString()
        {
            return Name + " " + Surname + ", " + Specialization;
        }
    }
}
