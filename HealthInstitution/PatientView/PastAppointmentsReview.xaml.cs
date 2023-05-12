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

namespace HealthInstitution.PatientView
{
    public partial class PastAppointmentsReview : Window
    {
        List<AppointmentReview> appointments;

        public PastAppointmentsReview()
        {
            InitializeComponent();
            appointments = GlobalRepository.schedule.FindPastForPatient(GlobalRepository.currentUser.Username)
                .Select(a => a.ToReview()).ToList();
            Table.ItemsSource = appointments;
        }

        private void SearchAnamnesis(object sender, RoutedEventArgs e)
        {
            Table.ItemsSource = appointments.Where(a => a.Anamnesis.Report.ToLower().Contains(SearchBox.Text.ToLower())).ToList();
        }

        private void SeeAnamnesis(object sender, RoutedEventArgs e)
        {
            AppointmentReview? selected = GetSelected();
            if (selected == null)
            {
                MessageBox.Show("Please select an appointment.", "Data missing",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AnamnesisReview window = new(selected.Anamnesis);
            window.Show();
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }

        private AppointmentReview? GetSelected()
        {
            var selectedIndex = Table.SelectedIndex;
            if (selectedIndex == -1) return null;
            return ((AppointmentReview)Table.Items[selectedIndex]);
        }
    }
}
