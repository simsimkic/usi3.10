using HealthInstitution.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Repository
{
    internal class DistributionRepository
    {
        private readonly string _filenameDistribution = "../../../Data/EquipmentRoom/distribution.json";

        public List<Distribution> Distributions;

        public DistributionRepository()
        {
            if (File.Exists(_filenameDistribution))
            {
                var distribution = (List<Distribution>?)FileLoader.Deserialize<Distribution>(_filenameDistribution);
                if (distribution != null)
                    Distributions = distribution;
            }

            if (Distributions == null)
                Distributions = new List<Distribution>();
        }

        public void Save()
        {
            FileLoader.Serialize<Distribution>(Distributions, _filenameDistribution);
        }

        public void Add(Distribution distribution)
        {
            Distributions.Add(distribution);
            Save();
        }

        public int GetReservedQuantity(int roomNumber, int equipmentId)
        {
            int quantity = 0;

            foreach(var item in Distributions)
            {
                if (item.IsCompleted == false && item.FromRoom == roomNumber)
                {
                    foreach (KeyValuePair<int, int> equipmentQuantity in item.EquipmentQuantity)
                    {
                        if (equipmentQuantity.Key == equipmentId)
                            quantity += equipmentQuantity.Value;
                    }
                }
            }

            return quantity;
        }

        public int GetOnWayQuantity(int roomNumber, int equipmentId)
        {
            int quantity = 0;

            foreach (var item in Distributions)
            {
                if (item.IsCompleted == false && item.ToRoom == roomNumber)
                {
                    foreach (KeyValuePair<int, int> equipmentQuantity in item.EquipmentQuantity)
                    {
                        if (equipmentQuantity.Key == equipmentId)
                            quantity += equipmentQuantity.Value;
                    }
                }
            }

            return quantity;
        }

        public List<Distribution> NewDistribution()
        {
            List<Distribution> list = new List<Distribution>();
            foreach (var distribution in Distributions)
            {
                if (!distribution.IsCompleted && distribution.Date <= DateTime.Now)
                {
                    distribution.IsCompleted = true;
                    list.Add(distribution);
                }
            }
            Save();
            return list;
        }
    }
}
