using HealthInstitution.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HealthInstitution.Repository
{
    class PurchaseRepository
    {
        private readonly string _filenamePurchase = "../../../Data/EquipmentRoom/purchases.json";

        public List<Purchase> Purchases;

        public PurchaseRepository()
        {
            if (File.Exists(_filenamePurchase))
            {
                var purchase = (List<Purchase>?)FileLoader.Deserialize<Purchase>(_filenamePurchase);
                if (purchase != null) Purchases = purchase;
            }

            if (Purchases == null)
                Purchases = new List<Purchase>();
        }

        public void Save()
        {
            FileLoader.Serialize<Purchase>(Purchases, _filenamePurchase);
        }

        public void Add(Purchase purchase)
        {
            Purchases.Add(purchase);
            Save();
        }

        public List<Purchase> NewPackages()
        {
            List<Purchase> list = new List<Purchase>();
            foreach (var purchase in Purchases)
            {
                if (DateTime.Now - purchase.Date > TimeSpan.FromDays(1) && !purchase.IsCompleted)
                {
                    purchase.IsCompleted = true;
                    list.Add(purchase);
                }
            }
            Save();
            return list;
        }

        public int OnTheWay(int equipmentId)
        {
            int retVal = 0;
            foreach (var purchase in Purchases)
            {
                if (DateTime.Now - purchase.Date <= TimeSpan.FromDays(1) && !purchase.IsCompleted)
                {
                    foreach (var el in purchase.Items)
                    {
                        if (el.Key == equipmentId)
                            retVal += el.Value;
                    }
                }
            }
            return retVal;
        }
    }
}
