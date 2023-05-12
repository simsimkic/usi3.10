using HealthInstitution.ManagerView;
using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.TableClass
{
    internal class EquipmentPurchaseReview
    {
        public int Id { get; set; }
        public bool Buy { get; set; }
        public string Equipment { get; set; }
        public string EquipmentType { get; set; }
        public int CurrentQuantity { get; set; }
        public int PurchaseQuantity { get; set; }
        public int OnTheWayQuantity { get; set; }

        public EquipmentPurchaseReview(int id, string equipment, string equipmentType, int currentQuantity, int purchaseQuantity, int onTheWayQuantity)
        {
            this.Id = id;
            this.Buy = false;
            this.Equipment = equipment;
            this.EquipmentType = equipmentType;
            this.CurrentQuantity = currentQuantity; 
            this.PurchaseQuantity = purchaseQuantity;
            this.OnTheWayQuantity = onTheWayQuantity;
        }
    }

    class EquipmentPurchaseTable
    {
        public ObservableCollection<EquipmentPurchaseReview> EquipmentPurchaseReviews { get; }

        public EquipmentPurchaseTable()
        {
            EquipmentPurchaseReviews = new ObservableCollection<EquipmentPurchaseReview>();
            var eqipmentRoomRepo = GlobalRepository.equipmentRoomRepository;
            var equipment = eqipmentRoomRepo.equipment;
            foreach (var el in equipment)
            {
                if (eqipmentRoomRepo.GetQuantity(el.Id) < 5 && (el.Type == Class.EquipmentType.Examination || el.Type == Class.EquipmentType.Operation))
                {
                    var equipmentTypeDesc = el.Type.GetType().GetMember(el.Type.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;
                    PurchaseRepository purchaseRepository = new PurchaseRepository();
                    var onTheWay = purchaseRepository.OnTheWay(el.Id);

                    var equipmentPurchase = new EquipmentPurchaseReview(el.Id, el.Name, equipmentTypeDesc.Description, eqipmentRoomRepo.GetQuantity(el.Id), 10, onTheWay);
                    EquipmentPurchaseReviews.Add(equipmentPurchase);
                }
            }
        }
    }
}
