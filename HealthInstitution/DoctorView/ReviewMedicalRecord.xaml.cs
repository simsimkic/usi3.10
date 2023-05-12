using HealthInstitution.Class;
using HealthInstitution.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace HealthInstitution.DoctorView
{
    public partial class ReviewMedicalRecord : Window
    {
        private Patient? SelectedPatient { get; set; }

        private Appointment? SelectedAppointment { get; set; }
        private bool IsPageForUpdate { get; set; }

        private bool IsPageForExamination { get; set; }

        private ObservableCollection<string> DiseasesBackup { get; set; }

        private ObservableCollection<string> AllergiesBackup { get; set; }


        public ReviewMedicalRecord(Appointment appointment, string patientUsername, bool isPageForUpdate, bool isPageForExamination = false)
        {
            InitializeComponent();

            SelectedPatient =  PatientRepository.FindPatient(patientUsername);
            SelectedAppointment = appointment;
            IsPageForUpdate = isPageForUpdate;
            IsPageForExamination = isPageForExamination;

            if (!isPageForExamination) 
                ShowAnamnesisButton.Visibility = Visibility.Hidden;

            if (!isPageForUpdate)
            {
                SetViewPageComponents();
                HideUpdatePageComponents();
            }
            else {
                SetUpdatePageComponents();
            }
            
        }

        public void Refresh(string patientUsername) {
            SelectedPatient = PatientRepository.FindPatient(patientUsername);
            SetBackupCollections();
            TableDiseases.ItemsSource = DiseasesBackup;
            TableAllergies.ItemsSource = AllergiesBackup;
        }

        private static bool IsValid(TextBox textBox) {
            string text = textBox.Text;
            if (string.IsNullOrEmpty(text)) return false;
            if (text.Length > 25 || text.Length < 2) return false;

            return Regex.IsMatch(text, @"^[a-zA-Z]+$");
        }

        private void SetViewPageComponents() {
            TableDiseases.ItemsSource = SelectedPatient.MedicalRecord.DiseaseHistory;
            TableAllergies.ItemsSource = SelectedPatient.MedicalRecord.Allergies;
            NameLabel.DataContext = SelectedPatient;
            SurnameLabel.DataContext = SelectedPatient;
            HeightLabel.DataContext = SelectedPatient.MedicalRecord;
            WeightLabel.DataContext = SelectedPatient.MedicalRecord;
        }

        private void SetUpdatePageComponents()
        {
            TextBoxSurname.Text = SelectedPatient.Surname;
            TextBoxName.Text = SelectedPatient.Name;
            SetBackupCollections();
            TableDiseases.ItemsSource = DiseasesBackup;
            TableAllergies.ItemsSource = AllergiesBackup;
            SetComboBoxItems();
        }

        private void SetBackupCollections() 
        {
            AllergiesBackup = new ObservableCollection<string>();
            DiseasesBackup = new ObservableCollection<string>();

            foreach (var item in SelectedPatient.MedicalRecord.DiseaseHistory)
                DiseasesBackup.Add(item);
            foreach (var item in SelectedPatient.MedicalRecord.Allergies)
                AllergiesBackup.Add(item);
        }

        private void SetComboBoxItems()
        {
            int[] comboItemsWeight = new int[150];
            for (int i = 0; i < 150; i++)
            {
                comboItemsWeight[i] = i + 10;
            }
            int[] comboItemsHeight = new int[130];
            for (int i = 0; i < 130; i++)
            {
                comboItemsHeight[i] = i + 100;
            }

            ComboBoxWeight.ItemsSource = comboItemsWeight;
            ComboBoxHeight.ItemsSource = comboItemsHeight;
            ComboBoxHeight.SelectedValue = (int)SelectedPatient.MedicalRecord.Height;
            ComboBoxWeight.SelectedValue = (int)SelectedPatient.MedicalRecord.Weight;

        }
        private void HideUpdatePageComponents() {
            ComboBoxHeight.Visibility = Visibility.Hidden;
            ComboBoxWeight.Visibility = Visibility.Hidden;
            TextBoxDiseases.Visibility = Visibility.Hidden;
            TextBoxAllergies.Visibility = Visibility.Hidden;
            AddAllergieButton.Visibility = Visibility.Hidden;
            AddDiseaseButton.Visibility = Visibility.Hidden;
            DeleteAllergieButton.Visibility = Visibility.Hidden;
            DeleteDiseaseButton.Visibility = Visibility.Hidden;
            SaveButton.Visibility = Visibility.Hidden;
            CancelButton.Visibility = Visibility.Hidden;
            TextBoxSurname.Visibility = Visibility.Hidden;
            TextBoxName.Visibility = Visibility.Hidden;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            if (!IsValid(TextBoxName)){ MessageBox.Show("Invalid data for name .", "Invalid data", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (!IsValid(TextBoxSurname)){ MessageBox.Show("Invalid data for surname .", "Invalid data", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            UpdateMedicalRecord();
            Close();
        }

        private void UpdateMedicalRecord() {
            int weight = (int)ComboBoxWeight.SelectedValue;
            int height = (int)ComboBoxHeight.SelectedValue;
            MedicalRecord newMedicalRecord = new MedicalRecord((double)height,
                (double)weight, TransferData(DiseasesBackup), TransferData(AllergiesBackup));

            Patient newPatient = new Patient(TextBoxName.Text, TextBoxSurname.Text, SelectedPatient.Username,
            SelectedPatient.Password, SelectedPatient.Blocked, newMedicalRecord);

            PatientRepository.UpdatePatient(SelectedPatient, newPatient);

        }

        private void AddAllergy(object sender, RoutedEventArgs e)
        {
            if (TextBoxAllergies.Text == "") return;
            if (IsDuplicate(AllergiesBackup, TextBoxAllergies.Text)) return;
            AllergiesBackup.Add(TextBoxAllergies.Text);
        }
        private void AddDisease(object sender, RoutedEventArgs e)
        {
            if (TextBoxDiseases.Text == "") return;
            if (IsDuplicate(DiseasesBackup, TextBoxDiseases.Text)) return;
            DiseasesBackup.Add(TextBoxDiseases.Text);
        }
        private void DeleteAllergy(object sender, RoutedEventArgs e)
        {
            if (TableAllergies.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a allergy to delete.", "Data missing",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                return ;
            }
            string allergy = (string)TableAllergies.Items[TableAllergies.SelectedIndex];
            AllergiesBackup.Remove(allergy);
        }
        private void DeleteDisease(object sender, RoutedEventArgs e)
        {
            if (TableDiseases.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a disease to delete.", "Data missing",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string allergy = (string)TableDiseases.Items[TableDiseases.SelectedIndex];
            DiseasesBackup.Remove(allergy);

        }

        private List<string> TransferData(ObservableCollection<string> list) {
            List<string> newList = new List<string>();
            foreach(var item in list)
            {
                newList.Add(item);
            }
            return newList;
        }

        private bool IsDuplicate(ObservableCollection<string> list, string text) { 
        
            foreach(var item in list)
            {
                if (item == text) return true;
            }
            return false;
        }

        private void ShowAnamnesis(object sender, RoutedEventArgs e)
        {
            UpdateMedicalRecord();
            NurseView.AnamnesisView window = new NurseView.AnamnesisView(SelectedAppointment, true, this);
            window.Show();
        }
    }
}
