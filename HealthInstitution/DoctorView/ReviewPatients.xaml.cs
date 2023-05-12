using HealthInstitution.Class;
using HealthInstitution.Repository;
using HealthInstitution.TableClass;
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

namespace HealthInstitution.DoctorView
{
    public partial class ReviewPatients : Window
    {
        public ReviewPatients()
        {
            InitializeComponent();
            PatientsTable.ItemsSource = PatientRepository.Patients;
        }

        private void Search(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var filtered = PatientRepository.Patients.Where(Patient => Patient.Username.ToLower().Contains(SearchBox.Text.ToLower())
                                                                       || Patient.Name.ToLower().Contains(SearchBox.Text.ToLower())
                                                                       || Patient.Surname.ToLower().Contains(SearchBox.Text.ToLower())
                                                                       || Patient.Blocked.ToString().ToLower().Contains(SearchBox.Text.ToLower()));
            PatientsTable.ItemsSource = filtered;
        }

        private string? GetSelectedUsername()
        {
            if (PatientsTable.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a patient.", "Data missing",
                MessageBoxButton.OK, MessageBoxImage.Warning);

                return null;
            }
            Patient patient = (Patient)PatientsTable.Items[PatientsTable.SelectedIndex];
            return patient.Username;
        }
        private void ShowMedicalRecord(object sender, RoutedEventArgs e)
        {
            string username = GetSelectedUsername();
            if (username == null) return;
            bool validPatient = GlobalRepository.schedule.ValidPatientForReview(username, GlobalRepository.currentUser.Username);

            if (!validPatient)
            {
                MessageBox.Show("You have never examinade this patient", "False option",
                MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }
            ReviewMedicalRecord window = new ReviewMedicalRecord(null, username, true);
            window.Show();
        }
    }
}
