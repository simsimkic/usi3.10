using HealthInstitution.Class;
using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace HealthInstitution.NurseView
{
    public partial class PatientAccountReview : Window
    {
        public Patient SelectedPatient { get; set; }
        public PatientAccountReview()
        {
            InitializeComponent();
            PatientsTable.ItemsSource = PatientRepository.Patients;
        }

        private void PatientsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPatient = (Patient)PatientsTable.SelectedItem;
        }

        private void OpenAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Patient oldPatient = SelectedPatient;
            Window accountDetailsView = new AccountDetailsView(SelectedPatient);

            if (accountDetailsView.ShowDialog() == true)
            {
                PatientRepository.UpdatePatient(oldPatient, SelectedPatient);
                PatientsTable.Items.Refresh();
            }
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPatient = new Patient();
            Window accountDetailsView = new AccountDetailsView(SelectedPatient);

            if (accountDetailsView.ShowDialog() == true)
            {
                PatientRepository.AddPatient(SelectedPatient);
                PatientsTable.Items.Refresh();
            }
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            PatientRepository.DeletePatient(SelectedPatient.Username);
            PatientsTable.Items.Refresh();
        }

    }
}
