using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HealthInstitution.Class
{
    public class MedicalRecord
    {
        public double Height { get; set; }
        public double Weight { get; set; }
        public List<string> DiseaseHistory { get; set; }
        public List<string> Allergies { get; set; }

        [JsonConstructor]
        public MedicalRecord(double height, double weight, List<string> diseaseHistory, List<string> allergies)
        {
            Height = height;
            Weight = weight;
            DiseaseHistory = diseaseHistory;
            Allergies = allergies;
        }

        public MedicalRecord()
        {
            DiseaseHistory = new List<string>();
            Allergies = new List<string>(); 
            Height = 0;
            Weight = 0;
        }

        public void UpdateLists(List<string> diseases, List<string> allergies) {

            foreach (var d in diseases) { DiseaseHistory.Add(d); }
            foreach (var d in allergies) { Allergies.Add(d); }
            IEnumerable<string> distinctAllergies = Allergies.Distinct();
            IEnumerable<string> distinctDiseases = DiseaseHistory.Distinct();
            DiseaseHistory = distinctDiseases.ToList();
            Allergies = distinctAllergies.ToList();
        }

        public override string ToString()
        {
            return $"H: {Height} W: {Weight} History: {DiseaseHistory}";
        }

        public string DiseaseHistoryToString()
        {
            string diseases = "";
            int i = 1;
            foreach (var disease in DiseaseHistory)
            {
                if (i++ == DiseaseHistory.Count)
                    diseases += disease;
                else
                    diseases += disease + ", ";
            }

            return diseases;
        }

        public string AllergiesToString()
        {
            int size = Allergies.Count;
            if (size == 0) return "";
            StringBuilder stringBuilder = new();
            for (int i = 0; i < size - 1; i++)
            {
                stringBuilder.Append(Allergies[i]);
                stringBuilder.Append(", ");
            }
            stringBuilder.Append(Allergies[size - 1]);
            return stringBuilder.ToString();
        }

        public List<string> DiseaseHistoryToList(string diseases)
        {
            String[] spearator = { "," };
            return diseases.Split(spearator, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
