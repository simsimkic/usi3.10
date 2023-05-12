using HealthInstitution.Class;
using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.TableClass
{
    public class EquipmentReview
    {
        public string Equipment { get; set; }
        public string EquipmentType { get; set; }
        public int Room { get; set; }
        public string RoomType { get; set;}
        public int Quantity { get; set; }

        public string EnteredQuantity { get; set; }

        public int EquipmentId { get; set; }

        public EquipmentReview(string equipment, string equipmentType, int room, string roomType, int quantity)
        {
            Equipment = equipment;
            EquipmentType = equipmentType;
            Room = room;
            RoomType = roomType;
            Quantity = quantity;
        }

        public EquipmentReview(int equipmentId, string equipment, string equipmentType, int quantity, string enteredQuantity)
        {
            EquipmentId = equipmentId;
            Equipment = equipment;
            EquipmentType = equipmentType;
            Quantity = quantity;
            EnteredQuantity = enteredQuantity;
        }
    }

    class EquipmentReviewTable
    {
        public ObservableCollection<EquipmentReview> EquipmentReviews { get; }

        public EquipmentReviewTable()
        {
            EquipmentReviews = new ObservableCollection<EquipmentReview>();
            var equipmentRoom = GlobalRepository.equipmentRoomRepository;
            foreach (var el in GlobalRepository.equipmentRoomRepository.equipmentRooms)
            {
                var equipment = equipmentRoom.FindEquipment(el.EquipmentID);
                var room = equipmentRoom.FindRoom(el.RoomID);

                var roomTypeDesc = room.Type.GetType().GetMember(room.Type.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;
                var equipmentTypeDesc = equipment.Type.GetType().GetMember(equipment.Type.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;

                var equipmentReview = new EquipmentReview(equipment.Name, equipmentTypeDesc.Description, room.Number, roomTypeDesc.Description, el.Quantity);
                EquipmentReviews.Add(equipmentReview);
            }
        }
    }
}
