using HealthInstitution.Class;
using HealthInstitution.TableClass;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.RightsManagement;
using System.Windows.Navigation;

namespace HealthInstitution.Repository
{
    public class EquipmentRoomRepository : INotifyPropertyChanged
    {
        public List<EquipmentRoom> equipmentRooms;
        private string _filenameEquipmentRooms = "../../../Data/EquipmentRoom/equipmentRoom.json";

        public List<Room> rooms;
        private string _filenameRooms = "../../../Data/EquipmentRoom/rooms.json";

        public List<Equipment> equipment;
        private string _filenameEquipment = "../../../Data/EquipmentRoom/equipment.json";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        

        public EquipmentRoomRepository()
        {
            equipmentRooms = (List<EquipmentRoom>)FileLoader.Deserialize<EquipmentRoom>(_filenameEquipmentRooms);
            rooms = (List<Room>)FileLoader.Deserialize<Room>(_filenameRooms);
            equipment = (List<Equipment>)FileLoader.Deserialize<Equipment>(_filenameEquipment);
        }

        public List<string> GetRooms()
        {
            List<string> roomNames = new List<string>();
            foreach (var room in rooms)
            {
                var roomTypeDesc = room.Type.GetType().GetMember(room.Type.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;
                roomNames.Add(room.Number.ToString() + " - " + roomTypeDesc.Description);
            }

            return roomNames;
        }

        public List<string> GetRoomsByQuantity()
        {
            List<string> roomNames = new List<string>();
            foreach (var item in equipmentRooms)
            {
                if (item.Quantity < 5)
                {
                    var equipment = FindEquipment(item.EquipmentID);
                    if (equipment.Type == EquipmentType.Operation || equipment.Type == EquipmentType.Examination)
                    {
                        var room = FindRoom(item.RoomID);
                        var roomTypeDesc = room.Type.GetType().GetMember(room.Type.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;

                        string name = item.RoomID.ToString() + " - " + roomTypeDesc.Description;
                        if (!roomNames.Contains(name))
                            roomNames.Add(name);
                    }
                }
            }
            roomNames.Sort();
            return roomNames;
        }
        public bool UpdateEquipmentQuantity(int quantity, int equipmentId, int roomId) {
            foreach(EquipmentRoom equipment in equipmentRooms)
            {
                if (equipment.EquipmentID == equipmentId && equipment.RoomID == roomId) {
                    equipment.Quantity -= quantity;
                    OnPropertyChanged("dynamicChanged");
                    SaveEquipmentRoom();
                    return true;
                }
            }
            return false;
        }

        public List<TableClass.EquipmentReview> GetEquipmentForDoctor(int roomNumber) { 
            List<TableClass.EquipmentReview> equipmentList = new List<TableClass.EquipmentReview> ();

            foreach (EquipmentRoom equipmentRoom in equipmentRooms) {
                if (equipmentRoom.RoomID == roomNumber) {
                    Equipment equipment = FindEquipment(equipmentRoom.EquipmentID);
                    if (IsDynamicEquipment(equipment))
                        equipmentList.Add(new TableClass.EquipmentReview(equipment.Id, equipment.Name, equipment.Type.ToString(), equipmentRoom.Quantity, "0"));
                }
            }
            return equipmentList;
        }

        private bool IsDynamicEquipment(Equipment equipment)
        {
            if (equipment.Type == EquipmentType.Operation || equipment.Type == EquipmentType.Examination)
                return true;
            return false;
        }
        public List<Room> GetRoomsOfType(RoomType type)
        {
            var selectedRooms = new List<Room>();
            foreach (var room in rooms)
                if (room.Type == type)
                    selectedRooms.Add(room);
            return selectedRooms;
        }

        public int GetReservedQuantity(int roomNumber, int equipmentId)
        {
            DistributionRepository distributionRepository = new DistributionRepository();
            return distributionRepository.GetReservedQuantity(roomNumber, equipmentId); 
        }

        public int GetOnWayQuantity(int roomNumber, int equipmentId)
        {
            DistributionRepository distributionRepository = new DistributionRepository();
            return distributionRepository.GetOnWayQuantity(roomNumber, equipmentId);
        }

        public int GetQuantity(int id)
        {
            int quantity = 0;
            foreach (var el in equipmentRooms)
            {
                if (el.EquipmentID == id)
                    quantity += el.Quantity;
            }

            return quantity;
        }

        public void CheckForNewEquipment()
        {
            PurchaseRepository purchaseRepository = new PurchaseRepository();
            
            foreach(var purchase in purchaseRepository.NewPackages())
            {
                foreach (KeyValuePair<int, int> item in purchase.Items)
                {
                    AddToWarehouse(item.Key, item.Value);
                }
                SaveEquipmentRoom();
            }
        }

        public void CheckForNewDistributions()
        {
            DistributionRepository distributionRepository = new DistributionRepository();

            foreach(var distribution in distributionRepository.NewDistribution())
            {
                MoveEquipment(distribution.FromRoom, distribution.ToRoom, distribution.EquipmentQuantity);
            }
        }

        public void MoveEquipment(int fromRoom, int toRoom, Dictionary<int, int> items)
        {
            foreach(KeyValuePair<int, int> item in items)
            {
                foreach(var equipRoom in equipmentRooms)
                {
                    if (equipRoom.RoomID == fromRoom && equipRoom.EquipmentID == item.Key)
                        equipRoom.Quantity -= item.Value;
                    if (equipRoom.RoomID == toRoom && equipRoom.EquipmentID == item.Key)
                        equipRoom.Quantity += item.Value;
                }
            }
            OnPropertyChanged("equipmentMoved");
            SaveEquipmentRoom();
        }

        public void CheckForNewChanges()
        {
            CheckForNewEquipment();
            CheckForNewDistributions();
        }

        private void AddToWarehouse(int id, int quantity)
        {
            foreach (var el in equipmentRooms)
            {
                if (el.RoomID == 101 && el.EquipmentID == id)
                {
                    el.Quantity += quantity;
                    OnPropertyChanged("warehouseChanged");
                    return;
                }
            }
        }

        public void SaveEquipmentRoom()
        {
            FileLoader.Serialize<EquipmentRoom>(equipmentRooms, _filenameEquipmentRooms);
        }

        public Room? FindRoom(int number)
        {
            foreach (var room in rooms)
            {
                if (room.Number == number)
                    return room;
            }

            return null;
        }

        public Equipment? FindEquipment(int id)
        { 
            foreach(var e in equipment)
            {
                if (e.Id == id)
                    return e;
            }
            return null;
        }

        public ObservableCollection<Equipment> GetAll()
        {
            var equipmentTable = new ObservableCollection<Equipment>();

            foreach (var e in equipment)
            {
                equipmentTable.Add(e);
            }

            return equipmentTable;
        }
    }
}
