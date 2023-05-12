using HealthInstitution.Class;
using HealthInstitution.PatientView;
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
    public partial class ReviewAppointmentsWindow : Window
    {
        public ReviewAppointmentsWindow()
        {
            InitializeComponent();
            DatePicker.SelectedDate = DateTime.Now;
            SetTable();
            
        }

        private void SetTable()
        {
            DateTime time = (DateTime)DatePicker.SelectedDate;
            Table.ItemsSource = GlobalRepository.schedule
                .FindUpcommingForDoctor(GlobalRepository.currentUser.Username)
                .Select(a => a.ToReviewDoctor(time))
                .Where(y => y != null).ToList();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void UpdateAppointment(object sender, RoutedEventArgs e)
        {
            var selectedAppointment = GetSelected();
            if (selectedAppointment == null) return;
            var window = new CreateAppointmentWindow(selectedAppointment.Id);
            window.Show();
        }

        private void CancelAppointment(object sender, RoutedEventArgs e)
        {
           var selectedAppointment = GetSelected();
            if (selectedAppointment == null) return;

            if (MessageBox.Show("Are you sure you want to cancel this examination?", "Cancel examination",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                GlobalRepository.schedule.CancelAppointment(selectedAppointment);
                SetTable();
            }
        }

        private void ShowMedicalRecord(object sender, RoutedEventArgs e)
        {
            var selectedAppointment = GetSelected();
            if (selectedAppointment == null) return;

            var window = new ReviewMedicalRecord(selectedAppointment, selectedAppointment.PatientUsername, false);
            window.Show();
        }

        private void HandleDateChange(object sender, SelectionChangedEventArgs e)
        {
            SetTable();
        }

        private void StartAppointment(object sender, RoutedEventArgs e)
        {
            var selectedAppointment = GetSelected();
            if (selectedAppointment == null) return;
            if (selectedAppointment.Status != AppointmentStatus.WAITING_FOR_DOCTOR) {
                MessageBox.Show("Patient hasn't been admissioned", "False option",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            ReviewMedicalRecord window = new ReviewMedicalRecord(selectedAppointment, selectedAppointment.PatientUsername, true, true);
            window.Show();
        }

        private Appointment? GetSelected()
        {
            var selectedIndex = Table.SelectedIndex;
            if (selectedIndex == -1)
            {
                MessageBox.Show("Please select row.", "Data missing",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            return ((AppointmentReview)Table.Items[selectedIndex]).ToAppointment();
        }
    }
}
