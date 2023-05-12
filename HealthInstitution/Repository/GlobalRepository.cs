using HealthInstitution.Class;
using System.ComponentModel;
using System.Diagnostics;

namespace HealthInstitution.Repository
{
    static class GlobalRepository
    {
        public static EquipmentRoomRepository equipmentRoomRepository = new EquipmentRoomRepository();
        public static User? currentUser = null;

        public static Schedule schedule = new Schedule();
        public static BackgroundWorker worker = new BackgroundWorker();
    }
}
