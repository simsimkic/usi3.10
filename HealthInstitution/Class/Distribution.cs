using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Class
{
    internal class Distribution
    {
        public int FromRoom { get; set; }
        public int ToRoom { get; set; }
        public Dictionary<int, int> EquipmentQuantity { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }

        public Distribution(int fromRoom, int toRoom, Dictionary<int, int> equipmentQuantity, DateTime date) {
            FromRoom = fromRoom;
            ToRoom = toRoom;
            Date = date;
            IsCompleted = false;
            EquipmentQuantity = equipmentQuantity;
        }
    }
}
