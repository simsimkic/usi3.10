using HealthInstitution.Class;
using HealthInstitution.Observer;
using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    public partial class AppointmentOptionsWindow : Window
    {

        private readonly List<Appointment> Suggestions;
        private readonly IObserver observer;

        public AppointmentOptionsWindow(CreateAppointmentBySuggestions window, List<Appointment> suggestions)
        {
            InitializeComponent();
            Suggestions = suggestions;
            observer = window;
            Option0.Content = suggestions[0].ToString();
            Option1.Content = suggestions[1].ToString();
            Option2.Content = suggestions[2].ToString();
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }

        private Appointment? GetSelected()
        {
            if (Option0.IsChecked == false && Option1.IsChecked == false && Option2.IsChecked == false)
            {
                MessageBox.Show("Pick an appointment.", "Data missing", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }
            if (Option0.IsChecked == true) return Suggestions[0];
            if (Option1.IsChecked == true) return Suggestions[1];
            return Suggestions[2];
        }

        private void Confirm(object sender, EventArgs e)
        {
            var selectedAppointment = GetSelected();
            if (selectedAppointment == null) return;

            AppointmentSuggestionService service = new();
            bool success = service.AddAppointment(selectedAppointment);
            if (success)
            {
                observer.Update();
                Close();
            }
            else
            {
                MessageBox.Show("Request failed because of invalid data.\nYou are not available at that time or there are no available rooms.",
                "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
