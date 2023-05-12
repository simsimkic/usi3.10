using HealthInstitution.Class;
using HealthInstitution.Observer;
using HealthInstitution.Repository;
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
    public partial class CreateAppointmentBySuggestions : Window, IObserver
    {
        public PatientInitialView observer;

        public CreateAppointmentBySuggestions(PatientInitialView window)
        {
            InitializeComponent();

            int[] comboItems = new int[24];
            for (int i = 0; i < 24; i++)
                comboItems[i] = i;

            ComboBoxHoursFrom.ItemsSource = comboItems;
            ComboBoxHoursFrom.SelectedIndex = 0;
            ComboBoxHoursTo.ItemsSource = comboItems;
            ComboBoxHoursTo.SelectedIndex = 0;

            string[] comboMinItems = { "00", "15", "30", "45" };
            ComboBoxMinutesFrom.ItemsSource = comboMinItems;
            ComboBoxMinutesFrom.SelectedIndex = 0;
            ComboBoxMinutesTo.ItemsSource = comboMinItems;
            ComboBoxMinutesTo.SelectedIndex = 0;

            Calendar.SelectedDate = DateTime.Now;
            Table.ItemsSource = DoctorRepository.Doctors;
            Table.SelectedIndex = 0;

            observer = window;
        }


        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
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

        private TimeOnly? GetSelectedTimeFrom()
        {
            if (ComboBoxHoursFrom.SelectedIndex == -1 || ComboBoxMinutesFrom.SelectedIndex == -1)
            {
                MessageBox.Show("Pick a time when you are available for the examination (from).", "Data missing",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            int hours = (int)ComboBoxHoursFrom.SelectedValue;
            int minutes = Convert.ToInt32(ComboBoxMinutesFrom.SelectedValue);
            return new TimeOnly(hours, minutes);
        }

        private TimeOnly? GetSelectedTimeTo()
        {
            if (ComboBoxHoursTo.SelectedIndex == -1 || ComboBoxMinutesTo.SelectedIndex == -1)
            {
                MessageBox.Show("Pick a time when you are available for the examination (to).", "Data missing",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            int hours = (int)ComboBoxHoursTo.SelectedValue;
            int minutes = Convert.ToInt32(ComboBoxMinutesTo.SelectedValue);
            return new TimeOnly(hours, minutes);
        }

        private Priority? GetSelectedPriority()
        {
            if (TimeIsPriority.IsChecked == false && DoctorIsPriority.IsChecked == false)
            {
                MessageBox.Show("Pick a priority.", "Data missing", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }
            return (bool)DoctorIsPriority.IsChecked ? Priority.DOCTOR : Priority.TIME;
        }

        private DateOnly? GetSelectedDate()
        {
            if (Calendar.SelectedDate == null)
            {
                MessageBox.Show("Pick a date.", "Data missing", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }
            return DateOnly.FromDateTime((DateTime)Calendar.SelectedDate);
        }

        private AppointmentScheduleRequest? GetDataFromForm()
        {
            DateOnly? end = GetSelectedDate();
            if (end == null) return null;

            TimeOnly? timeStart = GetSelectedTimeFrom();
            if (timeStart == null) return null;

            TimeOnly? timeEnd = GetSelectedTimeTo();
            if (timeEnd == null) return null;

            Doctor? doctor = GetSelectedDoctor();
            if (doctor == null) return null;

            Priority? priority = GetSelectedPriority();
            if (priority == null) return null;

            string patientUsername = GlobalRepository.currentUser.Username;
            try
            {
                TimeRange range = new(DateOnly.FromDateTime(DateTime.Now), (DateOnly)end,
                (TimeOnly)timeStart, (TimeOnly)timeEnd);
                return new(patientUsername, doctor.Username, range, (Priority)priority);
            }
            catch(ArgumentException)
            {
                MessageBox.Show("Time is not valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        private void CompleteAction(object sender, EventArgs e)
        {
            var request = GetDataFromForm();
            if (request == null) return;

            AppointmentSuggestionService service = new();
            List<Appointment> suggestions = service.GenerateSuggestion(request);

            if (suggestions.Count == 1)
            {
                if (MessageBox.Show("Suggested appointment: " + suggestions[0].ToString() +
                    ".\nWould you like to schedule this examination?", "Suggested Appointment",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;
                bool success = service.AddAppointment(suggestions[0]);
                if (success)
                    HandleSuccessfulAdd();
                else
                    MessageBox.Show("Request failed because of invalid data.\nYou are not available at that time or there are no available rooms.",
                    "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                AppointmentOptionsWindow selectionWindow = new(this, suggestions);
                selectionWindow.ShowDialog();
            }
        }

        private void HandleSuccessfulAdd()
        {
            NotifyObservers();
            MessageBox.Show("Success", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
            AppointmentSuggestionService service = new();
            if (service.PotentiallyBlock())
            {
                MessageBox.Show("Your account is now blocked. You have created 8 examination requests in the last 30 days.",
                    "Blocking your account", MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current.Shutdown();
            }
        }

        public void NotifyObservers()
        {
            observer.Update();
        }

        public void Update()
        {
            HandleSuccessfulAdd();
        }
    }
}
