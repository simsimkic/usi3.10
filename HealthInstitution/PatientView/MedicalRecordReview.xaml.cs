using HealthInstitution.Class;
using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HealthInstitution.PatientView
{
    public partial class MedicalRecordReview : Window
    {
        public MedicalRecordReview()
        {
            InitializeComponent();
            MedicalRecord medicalRecord = ((Patient)GlobalRepository.currentUser).MedicalRecord;
            Height.Content = medicalRecord.Height;
            Weight.Content = medicalRecord.Weight;
            Diseases.Text = medicalRecord.DiseaseHistoryToString();
            Allergens.Text = medicalRecord.AllergiesToString();
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }
    }
}
