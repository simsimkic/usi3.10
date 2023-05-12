using HealthInstitution.Class;
using HealthInstitution.Repository;
using Newtonsoft.Json.Linq;
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

namespace HealthInstitution.DoctorView
{
    public partial class EquipmentReview : Window
    {
        public Appointment SelectedAppointment { get; set; }
        public EquipmentReview(Appointment appointment)
        {
            InitializeComponent();
            SelectedAppointment = appointment;
            RoomNumberLabel.DataContext = appointment;
            EquipmentTable.ItemsSource = GlobalRepository.equipmentRoomRepository.GetEquipmentForDoctor(appointment.RoomNumber);
            GlobalRepository.equipmentRoomRepository.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "equipmentMoved" && e.PropertyName == "warehouseChanged")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    EquipmentTable.ItemsSource = GlobalRepository.equipmentRoomRepository.GetEquipmentForDoctor(SelectedAppointment.RoomNumber);
                });
            }
        }

        private void SubmitChanges(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < EquipmentTable.Items.Count; i++)
            {
                TableClass.EquipmentReview row = (TableClass.EquipmentReview)EquipmentTable.Items[i];
                string enteredQuantity = row.EnteredQuantity;
                if (!IsValidEnteredQuantity(enteredQuantity, row.Quantity)) return;
            }
            for (int i = 0; i < EquipmentTable.Items.Count; i++)
            {
                TableClass.EquipmentReview row = (TableClass.EquipmentReview)EquipmentTable.Items[i];
                GlobalRepository.equipmentRoomRepository.UpdateEquipmentQuantity(Int32.Parse(row.EnteredQuantity), row.EquipmentId, SelectedAppointment.RoomNumber);
            }
            Close();

        }

        public static bool IsValidEnteredQuantity(string enteredQuantity, int quantity) {
            if (!int.TryParse(enteredQuantity, out int number))
            {
                MessageBox.Show("Invalide type!!", "Data missing",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (quantity < Int32.Parse(enteredQuantity))
            {
                MessageBox.Show("Room doesn't have that much resources!!", "Data missing",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

    }
}
