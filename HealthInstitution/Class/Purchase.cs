using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Class
{
    internal class Purchase
    {
        public Dictionary<int, int> Items = new Dictionary<int, int>();
        public DateTime Date;
        public bool IsCompleted = false;

        public Purchase (Dictionary<int, int> items, DateTime date)
        {
            this.Items = items;
            this.Date = date;
        }
    }
}
