using HealthInstitution.Observer;
using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class PatientInitialView : Window, IObserver
    {

        private List<IObserver> observers = new();

        public PatientInitialView()
        {
            InitializeComponent();
            NameLabel.DataContext = GlobalRepository.currentUser;
            UsernameLabel.DataContext = GlobalRepository.currentUser;
            SurnameLabel.DataContext = GlobalRepository.currentUser;
            NameSurname.Content = "User: " + GlobalRepository.currentUser.Name
                + " "+ GlobalRepository.currentUser.Surname;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void CreateAppointment(object sender, RoutedEventArgs e)
        {
            CreateAppointmentWindowForPatient window = new(this);
            window.Show();
        }

        private void ReviewAppointments(object sender, RoutedEventArgs e)
        {
            ReviewAppointmentsForPatient window = new(this);
            window.Show();
            Subscribe(window);
        }

        private void CreateAppointmentWithPriority(object sender, RoutedEventArgs e)
        {
            CreateAppointmentBySuggestions window = new(this);
            window.Show();
        }

        public void OpenNotifications(object sender, RoutedEventArgs e)
        {
            NotificationsWindow window = new NotificationsWindow();
            window.Show();
        }
        private void ReviewRecord(object sender, RoutedEventArgs e)
        {
            MedicalRecordReview window = new();
            window.Show();
        }

        private void ReviewPastAppointments(object sender, RoutedEventArgs e)
        {
            PastAppointmentsReview window = new();
            window.Show();
        }

        private void LogOut(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.Hide();
            }
            MainWindow logIn = new();
            logIn.Show();
            
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }


        public void Update()
        {
            NotifyObservers();
        }

        public void Subscribe(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }
    }
}
