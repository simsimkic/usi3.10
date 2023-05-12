using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.TableClass
{
    internal class DistributionEquipmentReview : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Equipment { get; set; }
        public string EquipmentType { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int OnTheWayQuantity { get; set; }
        public int QuantityToMove { get; set; }

        public DistributionEquipmentReview(int id, string equipment, string equpimentType, int quantity, int reservedQuantity, int onTheWayQuantity, int quantityToMove)
        {
            Id = id;
            Equipment = equipment;
            EquipmentType = equpimentType;
            Quantity = quantity;
            ReservedQuantity = reservedQuantity;
            OnTheWayQuantity = onTheWayQuantity;
            QuantityToMove = quantityToMove;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    class DistributionEquipmentTable
    {
        public ObservableCollection<DistributionEquipmentReview> DistributionEquipmentReviews { get; }

        public DistributionEquipmentTable() {
            DistributionEquipmentReviews = new ObservableCollection<DistributionEquipmentReview>();
        }

        public void UpdateEquipment(int roomNumber, bool dynamic = false)
        {
            var equipmentRoomRepo = GlobalRepository.equipmentRoomRepository;
            var equipment = equipmentRoomRepo.equipment;
            var equipmentRooms = equipmentRoomRepo.equipmentRooms;
            DistributionEquipmentReviews.Clear();

            foreach (var item in equipmentRooms)
            {
                if (item.RoomID == roomNumber)
                {
                    var currentEquipment = equipmentRoomRepo.FindEquipment(item.EquipmentID);
                    if (dynamic == false && (currentEquipment.Type == Class.EquipmentType.Furniture || currentEquipment.Type == Class.EquipmentType.Corridor))
                    {
                        var equipmentTypeDesc = currentEquipment.Type.GetType().GetMember(currentEquipment.Type.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;

                        var distributionEquipmentReview = new DistributionEquipmentReview(item.EquipmentID, currentEquipment.Name, equipmentTypeDesc.Description, item.Quantity, equipmentRoomRepo.GetReservedQuantity(roomNumber, currentEquipment.Id), equipmentRoomRepo.GetOnWayQuantity(roomNumber, currentEquipment.Id), 0);
                        DistributionEquipmentReviews.Add(distributionEquipmentReview);
                    }
                    if (dynamic == true && (currentEquipment.Type == Class.EquipmentType.Operation || currentEquipment.Type == Class.EquipmentType.Examination))
                    {
                        var equipmentTypeDesc = currentEquipment.Type.GetType().GetMember(currentEquipment.Type.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;

                        var distributionEquipmentReview = new DistributionEquipmentReview(item.EquipmentID, currentEquipment.Name, equipmentTypeDesc.Description, item.Quantity, equipmentRoomRepo.GetReservedQuantity(roomNumber, currentEquipment.Id), equipmentRoomRepo.GetOnWayQuantity(roomNumber, currentEquipment.Id), 0);
                        DistributionEquipmentReviews.Add(distributionEquipmentReview);
                    }
                }
            }
        }
    }
}
