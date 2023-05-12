using HealthInstitution.ActionLog;
using HealthInstitution.Class;
using HealthInstitution.Repository;
using HealthInstitution.TableClass;
using Newtonsoft.Json.Linq;
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
    public partial class CreateAppointmentWindow : Window
    {

        private Appointment? SelectedAppointment { get; set; }
        public CreateAppointmentWindow()
        {
            Initialization();
        }

        public CreateAppointmentWindow(int appointmentId)
        {
            Initialization();
            SelectedAppointment = GlobalRepository.schedule.FindById(appointmentId);
            SetUpdatePageComponents();
        }
        private void Initialization()
        {
            InitializeComponent();

            ComboBoxHours.SelectedIndex = 0;
            ComboBoxMinutes.SelectedIndex = 0;
            ComboBoxType.SelectedIndex = 0;
            DatePicker.SelectedDate = DateTime.Now;

            int[] comboItems = new int[24];
            for (int i = 0; i < 24; i++)
            {
                comboItems[i] = i;
            }

            int[] comboItemsMinutes = new int[12];
            int count = 0;
            for (int i = 0; i < 12; i++)
            {
                comboItemsMinutes[i] = count;
                count += 5;
            }

            ComboBoxHours.ItemsSource = comboItems;
            ComboBoxDurationHours.ItemsSource = comboItems;
            ComboBoxDurationMinutes.ItemsSource = comboItemsMinutes;
            string[] comboMinItems = { "00", "15", "30", "45" };
            ComboBoxMinutes.ItemsSource = comboMinItems;
            string[] comboTypeItems = { "Examination", "Operation" };
            ComboBoxType.ItemsSource = comboTypeItems;
        }

        public void SetUpdatePageComponents() {
            Title = "Change Your Appointment";

            ComboBoxHours.SelectedValue = SelectedAppointment.TimeOf.Start.Hour;
            ComboBoxMinutes.SelectedValue = (SelectedAppointment.TimeOf.Start.Minute).ToString().PadLeft(2, '0');
            int type = (int)SelectedAppointment.Type;
            ComboBoxType.SelectedIndex = type;
            DatePicker.SelectedDate = SelectedAppointment.TimeOf.Start;

            if (SelectedAppointment.Type == AppointmentType.OPERATION)
            {
                ComboBoxDurationHours.SelectedValue = SelectedAppointment.TimeOf.DurationOf.TimeSpan.Hours;
                ComboBoxDurationMinutes.SelectedValue = SelectedAppointment.TimeOf.DurationOf.TimeSpan.Minutes;
            }

            var availablePatients = GlobalRepository.schedule.FindAvailablePatients(SelectedAppointment.TimeOf, SelectedAppointment.Id);

            Table.ItemsSource = availablePatients;
            Table.SelectedIndex = GetSelectedIndexPatient(availablePatients);
        }

        private int GetSelectedIndexPatient(List<Patient> patients) { 
            for(int i = 0; i < patients.Count; i++)
            {
                if (patients[i].Username == SelectedAppointment.PatientUsername) {
                    return i;
                }
            }
            return -1;
        }

        private Patient? GetSelectedPatient()
        {
            if (Table.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a patient.", "Data missing",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                
                return null;
            }
            return (Patient)Table.Items[Table.SelectedIndex];
        }

        private AppointmentType GetSelectedType() {
            return (AppointmentType)ComboBoxType.SelectedIndex;
        }

        protected void CompleteAction(object sender, EventArgs e)
        {
            var timeslot = GetSelectedTimeAndDuration();
            if (timeslot == null) return;

            var selectedPatient = GetSelectedPatient();
            if (selectedPatient == null) return;

            var selectedType = GetSelectedType();

            bool success;

            if (SelectedAppointment == null)
            {
                Appointment newAppointment = new(GlobalRepository.currentUser.Username,selectedPatient.Username, timeslot, selectedType);
                success = GlobalRepository.schedule.AddAppointment(newAppointment);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to change this examination?", "Change examination",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;

                success = GlobalRepository.schedule.UpdateAppointmentForDoctor(SelectedAppointment, timeslot, selectedPatient.Username, selectedType);

            }

            if (success)
            {
                Close();
                MessageBox.Show("Success", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Request failed because of invalid data.\n" +
                "Either the selected doctor is not available or you are not available at that time or there are no empty rooms." +
                ".\nTry using the button on the left.",
                "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void ShowAvailablePatients(object sender, RoutedEventArgs e)
        {

            var timeslot = GetSelectedTime();

            if (timeslot == null)
            {
                MessageBox.Show("Please select date and time",
                "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int id = 0;
            if (SelectedAppointment != null) {
                id = SelectedAppointment.Id;
            }

            bool isDoctorAvailable = GlobalRepository.schedule.IsDoctorAvailableFor(GlobalRepository.currentUser.Username, timeslot, id);
            if (!isDoctorAvailable)
            {
                MessageBox.Show("You are not available for selected date and time!",
                "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        
            var availablePatients = GlobalRepository.schedule.FindAvailablePatients(timeslot, id);

            Table.ItemsSource = availablePatients;
        }

        public Timeslot? GetSelectedTime()
        {
            if (ComboBoxHours.SelectedIndex == -1 || ComboBoxMinutes.SelectedIndex == -1)
            {
                MessageBox.Show("Pick a date and time for the examination.", "Data missing",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            DateTime start = (DateTime)DatePicker.SelectedDate;

            int hours = (int)ComboBoxHours.SelectedValue;
            int minutes = Convert.ToInt32(ComboBoxMinutes.SelectedValue);
            start = start.AddHours(hours).AddMinutes(minutes);

            return Timeslot.ForExamination(start);
        }

        public Timeslot? GetSelectedTimeAndDuration()
        {
            if (ComboBoxHours.SelectedIndex == -1 || ComboBoxMinutes.SelectedIndex == -1)
            {
                MessageBox.Show("Pick a date and time.", "Data missing",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            DateTime start = (DateTime)DatePicker.SelectedDate;

            int hours = (int)ComboBoxHours.SelectedValue;
            int minutes = Convert.ToInt32(ComboBoxMinutes.SelectedValue);
            start = start.AddHours(hours).AddMinutes(minutes);

            int durationHours = (int)ComboBoxDurationHours.SelectedValue;
            int durationMinutes = (int)ComboBoxDurationMinutes.SelectedValue;

            return Timeslot.ForOperation(start, durationHours, durationMinutes);
        }

        private void HandleTypeChange(object sender, System.EventArgs e)
        {
            string selectedType = (string)ComboBoxType.SelectedItem;

            if (selectedType == "Examination")
            {
                ComboBoxDurationHours.Visibility = Visibility.Hidden;
                ComboBoxDurationMinutes.Visibility = Visibility.Hidden;
                SelectDuration.Visibility = Visibility.Hidden;
                DurationHours.Visibility = Visibility.Hidden;
                DurationMinutes.Visibility = Visibility.Hidden;
                ComboBoxDurationHours.SelectedIndex = 0;
                ComboBoxDurationMinutes.SelectedIndex = 3;
            }
            else {
                ComboBoxDurationHours.Visibility = Visibility.Visible;
                ComboBoxDurationMinutes.Visibility = Visibility.Visible;
                SelectDuration.Visibility = Visibility.Visible;
                DurationHours.Visibility = Visibility.Visible;
                DurationMinutes.Visibility = Visibility.Visible;

            }
            
        }

        private void HandleDateChange(object sender, SelectionChangedEventArgs e)
        {
            Table.SelectedIndex = -1;
            Table.ItemsSource = new List<AppointmentReview>(); ;
        }

        private void HandleTimeChange(object sender, System.EventArgs e)
        {
            Table.SelectedIndex = -1;
            Table.ItemsSource = new List<AppointmentReview>(); ;
        }
    }


}
