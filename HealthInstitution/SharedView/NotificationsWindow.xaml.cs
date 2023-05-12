using HealthInstitution.Class;
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
    /// <summary>
    /// Interaction logic for NotificationsWindow.xaml
    /// </summary>
    public partial class NotificationsWindow : Window
    {
        public Notification SelectedNotification { get; set; }
        public NotificationsWindow()
        {
            InitializeComponent();

            NotificationsTable.ItemsSource = NotificationRepository.FindAllNotifications(GlobalRepository.currentUser.Username);

            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        private void NotificationsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedNotification = (Notification)NotificationsTable.SelectedItem;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (SelectedNotification != null)
                NotificationRepository.SetToRead(SelectedNotification.Id);
        }
    }
}
