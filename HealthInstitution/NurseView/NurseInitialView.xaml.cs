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

namespace HealthInstitution.NurseView
{
    /// <summary>
    /// Interaction logic for InitialView.xaml
    /// </summary>
    public partial class NurseInitialView : Window
    {
        public NurseInitialView()
        {
            InitializeComponent();
            NameLabel.DataContext = GlobalRepository.currentUser;
            UsernameLabel.DataContext = GlobalRepository.currentUser;
            SurnameLabel.DataContext = GlobalRepository.currentUser;
            NameSurnameDock.Content = GlobalRepository.currentUser.Name + " " + GlobalRepository.currentUser.Surname;
        }

        private void PatientAccountReview(object sender, RoutedEventArgs e)
        {
            PatientAccountReview window = new PatientAccountReview();
            window.Show();
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        
        private void PatientReviewMenu_Click(object sender, RoutedEventArgs e)
        {
            PatientAccountReview window = new PatientAccountReview();
            window.Show();
        }
        private void UrgentAppointmentMenu_Click(object sender, RoutedEventArgs e)
        {
            CreateUrgentAppointmentView window = new CreateUrgentAppointmentView();
            window.Show();
        }

        private void LogOutMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow window = new MainWindow();
            window.Show();
        }

    }
}
