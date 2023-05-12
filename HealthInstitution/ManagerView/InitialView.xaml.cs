using HealthInstitution.Class;
using HealthInstitution.Repository;
using System;
using System.Collections.Generic;
using System.Windows;

namespace HealthInstitution.ManagerView
{
    public partial class ManagerInitialView : Window
    {
        public ManagerInitialView()
        {
            InitializeComponent();
            GlobalRepository.equipmentRoomRepository.CheckForNewChanges();
            ShowInfo();
        }

        private void ShowInfo()
        {
            NameLabel.DataContext = GlobalRepository.currentUser;
            UsernameLabel.DataContext = GlobalRepository.currentUser;
            SurnameLabel.DataContext = GlobalRepository.currentUser;
            NameSurnameDock.Content = GlobalRepository.currentUser.Name + " " + GlobalRepository.currentUser.Surname;
        }

        private void EquipmentReview(object sender, RoutedEventArgs e)
        {
            EquipmentReview window = new EquipmentReview();
            window.Show();
        }

        private void PurchaseEquipment(object sender, RoutedEventArgs e)
        {
            EquipmentPurchase window = new EquipmentPurchase();
            window.Show();
        }

        private void DistributeStaticEquipment(object sender, RoutedEventArgs e)
        {
            DistributionStaticEquipment window = new DistributionStaticEquipment();
            window.Show();
        }

        private void DistributeDynamicEquipment(object sender, RoutedEventArgs e)
        {
            DistributionDynamicEquipemnt window = new DistributionDynamicEquipemnt();
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
    }
}
