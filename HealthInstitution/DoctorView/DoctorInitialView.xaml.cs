using HealthInstitution.PatientView;
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

namespace HealthInstitution.DoctorView
{
    public partial class DoctorInitialView : Window
    {
        public DoctorInitialView()
        {
            InitializeComponent();
            NameLabel.DataContext = GlobalRepository.currentUser;
            UsernameLabel.DataContext = GlobalRepository.currentUser;
            SurnameLabel.DataContext = GlobalRepository.currentUser;
            NameSurnameDock.Content = GlobalRepository.currentUser.Name + " " + GlobalRepository.currentUser.Surname;
        }

        private void CreateAppointment(object sender, RoutedEventArgs e)
        {
            CreateAppointmentWindow window = new CreateAppointmentWindow();
            window.Show();
        }

        private void ReviewAppointments(object sender, RoutedEventArgs e)
        {
            ReviewAppointmentsWindow window = new ReviewAppointmentsWindow();
            window.Show();
        }
        
        private void ReviewPatients(object sender, RoutedEventArgs e)
        {
            ReviewPatients window = new ReviewPatients();
            window.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
        private void Logout(object sender, RoutedEventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                w.Hide();
            }
            MainWindow window = new MainWindow();
            window.Show();
        }

        public void OpenNotifications(object sender, RoutedEventArgs e)
        {
            NotificationsWindow window = new NotificationsWindow();
            window.Show();
        }
    }
}
