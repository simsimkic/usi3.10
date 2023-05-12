using HealthInstitution.Class; 
using HealthInstitution.ManagerView;
using HealthInstitution.NurseView;
using HealthInstitution.DoctorView;

using HealthInstitution.Repository;
using System;
using System.Windows;
using HealthInstitution.PatientView;
using System.ComponentModel;
using System.Threading;

namespace HealthInstitution
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            LoginButton.Click += ButtonPress;

            CheckThread();
        }

        private void CheckThread()
        {
            GlobalRepository.worker.DoWork += (sender, e) =>
            {
                while (!GlobalRepository.worker.CancellationPending)
                {
                    GlobalRepository.equipmentRoomRepository.CheckForNewChanges();
                    Thread.Sleep(1000);
                }
            };

            if (!GlobalRepository.worker.IsBusy)
                GlobalRepository.worker.RunWorkerAsync();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void ButtonPress(object sender, RoutedEventArgs e)
        {
            var user = UserExists(Username.Text, Password.Text);
            if (user == null)
            {
                IncorrectText.Visibility = Visibility.Visible;
                return;
            }
            GlobalRepository.currentUser = user;

            if (user is Manager)
            {
                ManagerInitialView view = new ManagerInitialView();
                view.Show();
            }
            else if (user is Nurse)
            {
                NurseInitialView view = new NurseInitialView();
                    view.Show();
            }
            else if (user is Doctor)
            {
                DoctorInitialView view = new DoctorInitialView();
                view.Show();
            }
            else if (user is Patient)
            {
                if (((Patient)user).Blocked)
                {
                    MessageBox.Show("Your account is blocked. You cannot access it any longer.",
                        "Blocked account", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }
                PatientInitialView view = new PatientInitialView();
                view.Show();
            }
            Hide();
        }

        private static User? UserExists(string username, string password)
        {
            User? user = ManagerRepository.ManagerExists(username, password);
            if (user != null) return user;

            user = DoctorRepository.DoctorExists(username, password);
            if (user != null) return user;

            user = NurseRepository.NurseExists(username, password);
            if (user != null) return user;

            user = PatientRepository.PatientExists(username, password);
            return user;

        }
    }
}
