using HealthInstitution.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class AnamnesisReview : Window
    {

        public AnamnesisReview(Anamnesis anamnesis)
        {
            InitializeComponent();
            Symptoms.Text = anamnesis.SymptomsToString();
            Diseases.Text = anamnesis.DiseasesToString();
            Allergies.Text = anamnesis.AllergiesToString();
            Report.Text = anamnesis.Report;
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }
    }
}
