using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HealthInstitution.Class
{
    public class Room
    {
        public int Number { get; set; }
        public RoomType Type { get; set; }

    }

    public enum RoomType
    {
        [Description("Warehouse")] Warehouse,
        [Description("Operation room")] OperationRoom,
        [Description("Patient room")] PatientRoom,
        [Description("Exemination room")] ExaminationRoom,
        [Description("Waiting room")] WaitingRoom
    }
}
