using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Class
{
    public class Anamnesis
    {
        public List<string> Symptoms { get; set; }
        public List<string> DiseasesHistory { get;set; }
        public List<string> Allergies { get; set; }
        public string Report { get; set; }

        [JsonConstructor]
        public Anamnesis(List<string> symptoms, List<string> dieseases, List<string> allergies, string report) 
        {
            Symptoms = symptoms;
            DiseasesHistory = dieseases;
            Allergies = allergies;
            Report = report;
        }

        public Anamnesis()
        {
            Symptoms = new List<string>();
            DiseasesHistory = new List<string>();
            Allergies = new List<string>();
            Report = string.Empty;
        }

        public void Update(List<string> newSymptoms, List<string> newDiseases, List<string> newAllergies, string report)
        {
            Symptoms = newSymptoms;
            DiseasesHistory = newDiseases;
            Allergies = newAllergies;
            Report = report;
        }

        public string SymptomsToString()
        {
            int size = Symptoms.Count;
            if (size == 0) return "";

            StringBuilder sb = new();
            for (int i=0; i< Symptoms.Count-1; i++)
            {
                sb.Append(Symptoms[i]).Append(", ");
            }
            sb.Append(Symptoms[^1]);
            return sb.ToString();
        }

        public string DiseasesToString()
        {
            int size = DiseasesHistory.Count;
            if (size == 0) return "";
            StringBuilder sb = new();
            for (int i = 0; i < DiseasesHistory.Count - 1; i++)
            {
                sb.Append(DiseasesHistory[i]).Append(", ");
            }
            sb.Append(DiseasesHistory[^1]);
            return sb.ToString();
        }

        public string AllergiesToString()
        {
            int size = Allergies.Count;
            if (size == 0) return "";
            StringBuilder sb = new();
            for (int i = 0; i < Allergies.Count - 1; i++)
            {
                sb.Append(Allergies[i]).Append(", ");
            }
            sb.Append(Allergies[^1]);
            return sb.ToString();
        }
    }
}
