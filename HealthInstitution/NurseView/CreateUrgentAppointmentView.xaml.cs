using HealthInstitution.Class;
using HealthInstitution.Repository;
using HealthInstitution.TableClass;
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

namespace HealthInstitution.NurseView
{
    /// <summary>
    /// Interaction logic for CreateUrgentAppointmentView.xaml
    /// </summary>
    /// 


    public partial class CreateUrgentAppointmentView : Window, INotifyPropertyChanged
    {

        public List<string> TypeOfAppointmentComboItems { get; set; }
        public List<string> SpecializationComboItem { get; set; }
        public List<string> PatientsComboItems { get; set; }

        public AppointmentType SelectedTypeOfAppointment { get; set; }
        public string SelectedSpecialization { get; set; }
        public Patient SelectedPatient { get; set; }
        public Appointment SelectedAppointment { get; set; }
        public Appointment AppointmentToPostpone { get; set; }
        public int SelectedMinutes { get; set; }
        public int SelectedHours { get; set; }
        public List<Appointment> TopFiveToPostpone { get; set; }

        public UrgentAppointmentGenerator Generator { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private Visibility durationVisibility;
        public Visibility DurationVisibility
        {
            get { return durationVisibility;}
            set { 
                durationVisibility = value;
                OnPropertyChanged("DurationVisibility");
            }
        }


        public CreateUrgentAppointmentView()
        {
            InitializeComponent();

            Generator = new UrgentAppointmentGenerator();

            int[] comboItemsHours = new int[25];
            for (int i = 0; i <= 24; i++)
            {
                comboItemsHours[i] = i;
            }

            int[] comboItemsMinutes = new int[12];
            int count = 0;
            for (int i = 0; i < 12; i++)
            {
                comboItemsMinutes[i] = count;
                count += 5;
            }

            DurationVisibility = Visibility.Hidden;

            ComboBoxDurationHours.ItemsSource = comboItemsMinutes;
            ComboBoxDurationMinutes.ItemsSource = comboItemsMinutes;

            TypeOfAppointmentComboItems = new List<string> { "Examination", "Surgery" };
            SpecializationComboItem = DoctorRepository.GetAllSpecializations();
            PatientsComboItems = PatientRepository.PatientsToStrings();

            ComboBoxSpecialization.ItemsSource = SpecializationComboItem;
            ComboBoxType.ItemsSource = TypeOfAppointmentComboItems;
            ComboBoxPatient.ItemsSource = PatientsComboItems;

            TopFiveToPostpone = new List<Appointment>();

            DataContext = this;
        }

        private void ComboBoxPatient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedPatientUsername = ((string)ComboBoxPatient.SelectedItem).Split(' ').Last();
            SelectedPatient = PatientRepository.FindPatient(selectedPatientUsername);
            TxtBlockPatient.Text = "Patient: " + (string)ComboBoxPatient.SelectedItem;
        }

        private void ComboBoxSpecialization_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSpecialization = (string)ComboBoxSpecialization.SelectedItem;
            TxtBlockSpecializatioin.Text = SelectedSpecialization.ToUpper();
        }

        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTypeOfAppointment = ((string)ComboBoxType.SelectedItem == "Examination") ? 
                AppointmentType.EXAMINATION : AppointmentType.OPERATION;

            if (SelectedTypeOfAppointment == AppointmentType.OPERATION)
                DurationVisibility = Visibility.Visible;
            else
                DurationVisibility = Visibility.Hidden;
           


            TxtBlockType.Text = ((string)ComboBoxType.SelectedItem).ToUpper();
        }

        private void FindAvailableAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTypeOfAppointment == AppointmentType.EXAMINATION)
                SelectedAppointment = Generator.FindUrgentAppointment(
                    SelectedPatient, SelectedTypeOfAppointment, SelectedSpecialization);
            else
                SelectedAppointment = Generator.FindUrgentAppointment(
                    SelectedPatient, SelectedTypeOfAppointment, SelectedSpecialization,
                    SelectedHours * 60 + SelectedMinutes);

            if (SelectedAppointment != null)
                FillAppointmentDetails(SelectedAppointment);
            else
            {
                GenerateAppointmentList();
                MessageBox.Show("There are not available appointments in the next 2 hours.\n" +
                    "Please, select appointment to postpone.", "Notiffication", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            if(AppointmentToPostpone != null)
            {
                //Postpone appointment
                GlobalRepository.schedule.UpdateStatus(
                        AppointmentToPostpone, AppointmentStatus.CANCELED
                    );

                //Add new urgent appointment
                SelectedAppointment = new Appointment(AppointmentToPostpone.DoctorUsername, 
                    SelectedPatient.Username, AppointmentToPostpone.TimeOf, SelectedTypeOfAppointment);

                //Schedule new appointment instead of postponed 
                Appointment insteadOfPostponed = Generator.FindFirstNextAppointment(AppointmentToPostpone);
                GlobalRepository.schedule.AddAppointment(insteadOfPostponed);


                //Notify others
                SendNotificationsAboutPostponement(insteadOfPostponed);
            }
            
            if (SelectedAppointment == null)
            {
                MessageBox.Show("Appointment is not selected!", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (GlobalRepository.schedule.AddAppointment(SelectedAppointment))
            {
                MessageBox.Show("Urgent appointment is successfuly added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                SendNotificationsToDoctor();
                this.Close();
            }
            else
            {
                MessageBox.Show("This patient already have scheduled apoointment.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
                 
        }

        private void GenerateAppointmentList()
        {
            var appointmentsToPostpone = Generator.FindAppointmentsToPostpone();
            AppointmentsTable.ItemsSource = appointmentsToPostpone.Select(a => a.ToReview());
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ComboBoxDurationHours_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedHours = (int)ComboBoxDurationHours.SelectedItem;
        }

        private void ComboBoxDurationMinutes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedMinutes = (int)ComboBoxDurationMinutes.SelectedItem;
        }

        private void AppointmentsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppointmentToPostpone = ((AppointmentReview)AppointmentsTable.SelectedItem).ToAppointment();
            FillAppointmentDetails(AppointmentToPostpone);
        }

        public void FillAppointmentDetails(Appointment appointment)
        {
            TxtBlockDate.Text = "Date: " + appointment.TimeOf.Start.ToString();

            TxtBlockDuration.Text = "Duration: " + appointment.TimeOf.DurationOf.ToString();

            var doctor = DoctorRepository.FindDoctor(appointment.DoctorUsername);
            TxtBlockDoctor.Text = "Doctor: " + doctor.Name + " " + doctor.Surname + " " + doctor.Username;

        }

        public void SendNotificationsAboutPostponement(Appointment newAppointment)
        {
            string messageForDoctor = "NEW URGENT APPOINTMENT. You have new urgent appointment scheduled for "
                + SelectedAppointment.TimeOf.Start.ToShortTimeString() + 
                " instead of the appointment at " + AppointmentToPostpone.TimeOf.Start.ToShortTimeString();

            string messageForPatient = "RESCHEDULED APPOINTMENT. Your appointment at " + AppointmentToPostpone.TimeOf.ToString() + " is canceled. "+
                "Your new appointment is scheduled for " + newAppointment.TimeOf.Start.ToShortTimeString();

            NotificationRepository.AddNotification(new Notification(messageForDoctor, AppointmentToPostpone.DoctorUsername));
            NotificationRepository.AddNotification(new Notification(messageForPatient, AppointmentToPostpone.PatientUsername));
        }

        public void SendNotificationsToDoctor()
        {
            string messageForDoctor = "NEW URGENT APPOINTMENT. You have new urgent appointment scheduled for "
                + SelectedAppointment.TimeOf.Start.ToShortTimeString();

            NotificationRepository.AddNotification(new Notification(messageForDoctor, SelectedAppointment.DoctorUsername));
        }
    }
}
