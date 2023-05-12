using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Class
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EquipmentType Type { get; set; }
    }

    public enum EquipmentType
    {
        [Description("Examination equipment")] Examination,
        [Description("Operation equipment")] Operation,
        [Description("Room furniture")] Furniture,
        [Description("Corridor equipment")] Corridor
    }
}
