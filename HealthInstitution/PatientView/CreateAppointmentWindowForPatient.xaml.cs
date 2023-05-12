using HealthInstitution.Class;
using HealthInstitution.ActionLog;
using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using HealthInstitution.Observer;
using HealthInstitution.TableClass;

namespace HealthInstitution.PatientView
{

    public partial class CreateAppointmentWindowForPatient : Window
    {

        //This is null when creating a new appointment and not null when editing an existing appointment
        private Appointment? SelectedAppointment { get; set; }

        private IObserver observer;


        public CreateAppointmentWindowForPatient(PatientInitialView patientInitialView)
        {
            Initialization();
            ComboBoxHours.SelectedIndex = 0;
            ComboBoxMinutes.SelectedIndex = 0;
            DatePicker.SelectedDate = DateTime.Now;
            Table.ItemsSource = DoctorRepository.Doctors;
            Table.SelectedIndex = 0;
            observer = patientInitialView;
        }


        //For updating an existing appointment
        public CreateAppointmentWindowForPatient(PatientInitialView patientInitialView, int appointmentId)
        {
            Initialization();

            Title = "Change Your Appointment";

            SelectedAppointment = GlobalRepository.schedule.FindById(appointmentId);

            ComboBoxHours.SelectedValue = SelectedAppointment.TimeOf.Start.Hour;
            ComboBoxMinutes.SelectedValue = (SelectedAppointment.TimeOf.Start.Minute).ToString().PadLeft(2, '0');
            DatePicker.SelectedDate = SelectedAppointment.TimeOf.Start;
            Table.ItemsSource = GlobalRepository.schedule.FindAvailableDoctors(SelectedAppointment.TimeOf);
            Table.SelectedIndex = 0;
            observer = patientInitialView;
        }


        private void Initialization()
        {
            InitializeComponent();

            int[] comboItems = new int[24];
            for (int i = 0; i < 24; i++)
                comboItems[i] = i;

            ComboBoxHours.ItemsSource = comboItems;

            string[] comboMinItems = { "00", "15", "30", "45" };
            ComboBoxMinutes.ItemsSource = comboMinItems;
        }


        protected void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }


        protected void CompleteAction(object sender, EventArgs e)
        {
            var timeslot = GetSelectedTime();
            if (timeslot == null) return;

            var selectedDoctor = GetSelectedDoctor();
            if (selectedDoctor == null) return;

            bool success;

            if (SelectedAppointment == null)
            {
                Appointment newAppointment = new(selectedDoctor.Username,
                    GlobalRepository.currentUser.Username, timeslot, AppointmentType.EXAMINATION);

                success = GlobalRepository.schedule.AddAppointment(newAppointment);
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to change this examination?", "Change examination",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;

                success = GlobalRepository.schedule.UpdateAppointment(SelectedAppointment, timeslot, selectedDoctor.Username);
            }

            if (success)
            {
                NotifyObservers();
                Close();
                MessageBox.Show("Success", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                PotentiallyBlock();
            }
            else
            {
                MessageBox.Show("Request failed because of invalid data.\n" +
                "Either the selected doctor is not available or you are not available at that time" +
                ".\nTry using the button on the left.",
                "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Doctor? GetSelectedDoctor()
        {
            if (Table.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a doctor.", "Data missing",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            return (Doctor)Table.Items[Table.SelectedIndex];
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


        protected void ShowAvailableDoctors(object sender, EventArgs e)
        {
            var timeslot = GetSelectedTime();
            if (timeslot == null) return;

            Table.ItemsSource = GlobalRepository.schedule.FindAvailableDoctors(timeslot);
        }


        private void PotentiallyBlock()
        {
            string patientUsername = GlobalRepository.currentUser.Username;
            bool readyForBlocking;
            if (SelectedAppointment == null)
            {
                Log.AddItem(new(patientUsername, ActionLog.Action.CREATE));
                readyForBlocking = Log.CheckCreateRequests(patientUsername);
            }
            else
            {
                Log.AddItem(new(patientUsername, ActionLog.Action.EDIT));
                readyForBlocking = Log.CheckEditDeleteRequests(patientUsername);
            }

            if (readyForBlocking)
            {
                PatientRepository.Block((Patient)GlobalRepository.currentUser);
                MessageBox.Show("Your account is now blocked. You have created 8 examination requests or you have deleted" +
                    " or edited your existing examinations for at least 5 times in the last 30 days.", "Blocking your account",
                    MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current.Shutdown();
            }
        }


        public void NotifyObservers()
        {
            observer.Update();
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
