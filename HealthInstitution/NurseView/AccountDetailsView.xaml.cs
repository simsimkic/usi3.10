using HealthInstitution.Class;
using HealthInstitution.Repository;
using HealthInstitution.TableClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// <summary>
    /// Interaction logic for AccountDetailsView.xaml
    /// </summary>
    public partial class AccountDetailsView : Window, INotifyPropertyChanged
    {
        public string NameP { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public double HeightP { get; set; }
        public double Weight { get; set; }

        public Patient SelectedPatient { get; set; }

        public List<string> DiseaseHistory { get; set; }
        public List<string> Allergies { get; set; }

        public Appointment SelectedAppointment { get; set; }

        private ObservableCollection<AppointmentReview> _appointmentTableItems;
        public ObservableCollection<AppointmentReview> AppointmentTableItems
        {
            get { return _appointmentTableItems; }
            set
            {
                if (_appointmentTableItems != value)
                {
                    _appointmentTableItems = value;
                    OnPropertyChanged(nameof(AppointmentTableItems));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public AccountDetailsView(Patient patient)
        {
            InitializeComponent();

            SelectedPatient = patient;

            AppointmentTableItems = new ObservableCollection<AppointmentReview>();

            var items = GlobalRepository.schedule
                 .FindForPatient(SelectedPatient.Username)
                 .Select(a => a.ToReview()).ToList();

            foreach (var item in items)
            {
                AppointmentTableItems.Add(item);
            }

            NameP = patient.Name;
            Surname = patient.Surname;
            Username = patient.Username;
            Password = patient.Password;
            HeightP = patient.MedicalRecord.Height;
            Weight = patient.MedicalRecord.Weight;
            DiseaseHistory = patient.MedicalRecord.DiseaseHistory;
            Allergies = patient.MedicalRecord.Allergies;

            DataContext = this;

        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientRepository.IsUniqueUsername(UsernameT.Text) &&
                PatientRepository.IsValidName(NameT.Text) &&
                PatientRepository.IsValidSurname(SurnameT.Text) &&
                PatientRepository.IsValidPassword(PasswordT.Text) &&
                PatientRepository.IsValidWeight(WeightT.Text) &&
                PatientRepository.IsValidHeight(HeightT.Text)
                )
            {
                SelectedPatient.Name = NameT.Text;
                SelectedPatient.Surname = SurnameT.Text;
                SelectedPatient.Username = UsernameT.Text;
                SelectedPatient.Password = PasswordT.Text;
                SelectedPatient.MedicalRecord.Height = Convert.ToDouble(HeightT.Text);
                SelectedPatient.MedicalRecord.Weight = Convert.ToDouble(WeightT.Text);
                SelectedPatient.MedicalRecord.DiseaseHistory = DiseaseHistory;
                SelectedPatient.MedicalRecord.Allergies = Allergies;
            
                this.DialogResult = true;

            }
            else
            {
                MessageBox.Show("Invalid data", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void AddDiseaseButton_Click(object sender, RoutedEventArgs e)
        {
            DiseaseHistory.Add(DiseaseInputT.Text);
            DiseaseHistoryListBox.Items.Refresh();
            DiseaseInputT.Text = "";

        }

        private void AddAllergyButton_Click(object sender, RoutedEventArgs e)
        {
            Allergies.Add(AllergyInputT.Text);
            AllergiesListBox.Items.Refresh();
            AllergyInputT.Text = "";
        }

        private void ShowAnamnesisButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidForAdmission()) return;
            
            AnamnesisView window = new AnamnesisView(SelectedAppointment, false, null);
            if (window.ShowDialog() == true)
            {
                var appointment = AppointmentTableItems.FirstOrDefault(x => x.Id == SelectedAppointment.Id);
                if (appointment != null)
                {
                    appointment.Status = AppointmentStatus.WAITING_FOR_DOCTOR;
                    AppointmentsTable.Items.Refresh();
                }

            }     
                  
        }

        private void AppointmentsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAppointment = ((AppointmentReview)AppointmentsTable.SelectedItem).ToAppointment();
        }

        public bool IsValidForAdmission()
        {
            if (SelectedAppointment == null)
            {
                MessageBox.Show("Appointment is not selected!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!SelectedAppointment.TimeOf.IsValidTimeForAdmission())
            {
                MessageBox.Show("Patient addmission must start at least 15 minutes before!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


            if (SelectedAppointment.Status != AppointmentStatus.ACTIVE)
            {
                MessageBox.Show("Appointment is not active!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

    }
}
