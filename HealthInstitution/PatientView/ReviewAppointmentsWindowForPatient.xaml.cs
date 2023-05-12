using HealthInstitution.ActionLog;
using HealthInstitution.Class;
using HealthInstitution.Observer;
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

    public partial class ReviewAppointmentsForPatient : Window, IObserver
    {

        private PatientInitialView PatientInitialView;

        public ReviewAppointmentsForPatient(PatientInitialView initialWindow)
        {
            InitializeComponent();
            SetTable();
            PatientInitialView = initialWindow;
        }

        private void SetTable() {
            Table.ItemsSource = GlobalRepository.schedule
                .FindUpcommingForPatient(GlobalRepository.currentUser.Username)
                .Select(a => a.ToReview());
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
            PatientInitialView.Unsubscribe(this);
        }


        private void UpdateAppointment(object sender, EventArgs e)
        {
            var selectedAppointment = GetSelected();
            if (!ValidForModification(selectedAppointment)) return;
            var window = new CreateAppointmentWindowForPatient(PatientInitialView, selectedAppointment.Id);
            window.Show();
        } 


        private void CancelAppointment(object sender, EventArgs e)
        {
            var selectedAppointment = GetSelected();
            if (!ValidForModification(selectedAppointment)) return;

            if (MessageBox.Show("Are you sure you want to cancel this examination?", "Cancel examination",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                GlobalRepository.schedule.CancelAppointment(selectedAppointment);

                PotentiallyBlock();
                SetTable();
            }
        }

        private Appointment? GetSelected()
        {
            var selectedIndex = Table.SelectedIndex;
            if (selectedIndex == -1) return null;
            return ((AppointmentReview)Table.Items[selectedIndex]).ToAppointment();
        }

        private static bool ValidForModification(Appointment? selectedAppointment)
        {
            if (selectedAppointment == null)
            {
                MessageBox.Show("Select an appointment.", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (selectedAppointment.Status == AppointmentStatus.CANCELED)
            {
                MessageBox.Show("This appointment is already cancelled.",
                    "Already cancelled", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!selectedAppointment.ValidForUpdate())
            {
                MessageBox.Show("Changing an appointment has to be done at least 24h earlier.",
                    "Too late", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }


        private static void PotentiallyBlock()
        {
            string patientUsername = GlobalRepository.currentUser.Username;
            Log.AddItem(new(patientUsername, ActionLog.Action.DELETE));
            bool readyForBlocking = Log.CheckEditDeleteRequests(patientUsername);
            if (readyForBlocking)
            {
                PatientRepository.Block((Patient)GlobalRepository.currentUser);
                MessageBox.Show("Your account is now blocked. You have created 8 examination requests or you have deleted" +
                    " or edited your existing examinations for at least 5 times in the last 30 days.", "Blocking your account",
                    MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current.Shutdown();
            }
        }

        public void Update()
        {
            SetTable();
        }
    }
}
