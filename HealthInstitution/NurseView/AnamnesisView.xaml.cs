using HealthInstitution.Class;
using HealthInstitution.DoctorView;
using HealthInstitution.Repository;
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
    public partial class AnamnesisView : Window
    {
        public Appointment SelectedAppointment { get; set; }

        public List<string> Diseases { get; set; }
        public List<string> Allergies { get; set; }
        public List<string> Symptoms { get; set; }
        public bool IsDoctorEditing { get; set; }

        public ReviewMedicalRecord MedicalRecordWindow { get; set; }

        public AnamnesisView(Appointment selectedAppoinment, bool isDoctorEditing, ReviewMedicalRecord medicalRecordWindow)
        {
            InitializeComponent();
            MedicalRecordWindow = medicalRecordWindow;
            IsDoctorEditing = isDoctorEditing;
            if (!isDoctorEditing) DoctorReport.Visibility = Visibility.Hidden;
            if(isDoctorEditing) DoctorReport.Text = selectedAppoinment.Anamnesis.Report;
            SelectedAppointment = selectedAppoinment;
            Diseases = SelectedAppointment.Anamnesis.DiseasesHistory;
            Allergies = SelectedAppointment.Anamnesis.Allergies;
            Symptoms = SelectedAppointment.Anamnesis.Symptoms;

            DataContext = this;

        }
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsDoctorEditing)
            {
                DoctorView.EquipmentReview window = new DoctorView.EquipmentReview(SelectedAppointment);
                window.Show();
                PatientRepository.UpdateMedicalRecord(Diseases, Allergies, SelectedAppointment.PatientUsername);
                GlobalRepository.schedule.UpdateStatus(SelectedAppointment, AppointmentStatus.FINISHED);
                GlobalRepository.schedule.UpdateAppointmenAnamnesis(SelectedAppointment, Symptoms, Diseases, Allergies, DoctorReport.Text);
                MedicalRecordWindow.Refresh(SelectedAppointment.PatientUsername);
            }
            else {
                GlobalRepository.schedule.UpdateStatus(SelectedAppointment, AppointmentStatus.WAITING_FOR_DOCTOR);
                GlobalRepository.schedule.UpdateAppointmenAnamnesis(SelectedAppointment, Symptoms, Diseases, Allergies, DoctorReport.Text);
                this.DialogResult = true;
            }
            
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddDiseaseButton_Click(object sender, RoutedEventArgs e)
        {
            Diseases.Add(NewDiseaseT.Text);
            DiseasesListBox.Items.Refresh();
            NewDiseaseT.Text = "";
        }


        private void AddAllergyButton_Click(object sender, RoutedEventArgs e)
        {
            Allergies.Add(NewAllergyT.Text);
            AllergiesListBox.Items.Refresh();
            NewAllergyT.Text = "";
        }

        private void AddSymptomButton_Click(object sender, RoutedEventArgs e)
        {
            Symptoms.Add(NewSymptomT.Text);
            SymptomsListBox.Items.Refresh();
            NewSymptomT.Text = "";
        }

        private void DeleteSymptomButton_Click(object sender, RoutedEventArgs e)
        {
            if (SymptomsListBox.SelectedItem == null) return;
            Symptoms.Remove(SymptomsListBox.SelectedItem.ToString());
            SymptomsListBox.Items.Refresh();
        }

        private void DeleteDiseaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (DiseasesListBox.SelectedItem == null) return;
            Diseases.Remove(DiseasesListBox.SelectedItem.ToString());
            DiseasesListBox.Items.Refresh();

        }

        private void DeleteAllergyButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllergiesListBox.SelectedItem == null) return;
            Allergies.Remove(AllergiesListBox.SelectedItem.ToString());
            AllergiesListBox.Items.Refresh();
        }
    }
}
