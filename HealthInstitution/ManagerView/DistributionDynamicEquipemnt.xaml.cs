using HealthInstitution.Class;
using HealthInstitution.Repository;
using HealthInstitution.TableClass;

using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HealthInstitution.ManagerView
{
    public partial class DistributionDynamicEquipemnt : Window
    {
        DistributionEquipmentTable reviewsFrom = new DistributionEquipmentTable();
        DistributionEquipmentTable reviewsTo = new DistributionEquipmentTable();
        int roomNumberFrom, roomNumberTo;
        public DistributionDynamicEquipemnt()
        {
            InitializeComponent();
            FromRoom.ItemsSource = GlobalRepository.equipmentRoomRepository.GetRooms();
            ToRoom.ItemsSource = GlobalRepository.equipmentRoomRepository.GetRoomsByQuantity();
            GlobalRepository.equipmentRoomRepository.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "dynamicChanged" || e.PropertyName == "warehouseChanged")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    reviewsFrom.UpdateEquipment(roomNumberFrom);
                    reviewsTo.UpdateEquipment(roomNumberTo);
                });
            }
        }

        private void FromRoomChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FromRoom.SelectedItem != null)
            {
                roomNumberFrom = int.Parse(FromRoom.SelectedItem.ToString().Split(" - ")[0]);
                if (ToRoom.SelectedItem == null || (ToRoom.SelectedItem != null && int.Parse(ToRoom.SelectedItem.ToString().Split(" - ")[0]) != roomNumberFrom))
                {
                    reviewsFrom.UpdateEquipment(roomNumberFrom, true);
                    FromTable.ItemsSource = reviewsFrom.DistributionEquipmentReviews;
                }
                else
                {
                    FromRoom.SelectedItem = null;
                    MessageBox.Show("Please select two different rooms.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ToRoomChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToRoom.SelectedItem != null)
            {
                roomNumberTo = int.Parse(ToRoom.SelectedItem.ToString().Split(" - ")[0]);
                if (FromRoom.SelectedItem == null || (FromRoom.SelectedItem != null && int.Parse(FromRoom.SelectedItem.ToString().Split(" - ")[0]) != roomNumberTo))
                {
                    reviewsTo.UpdateEquipment(roomNumberTo, true);
                    ToTable.ItemsSource = reviewsTo.DistributionEquipmentReviews;
                }
                else
                {
                    ToRoom.SelectedItem = null;
                    MessageBox.Show("Please select two different rooms.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Distribute(object sender, RoutedEventArgs e)
        {
            if (FromRoom.SelectedItem == null || ToRoom.SelectedItem == null)
                MessageBox.Show("Please select rooms and date.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                roomNumberFrom = int.Parse(FromRoom.SelectedItem.ToString().Split(" - ")[0]);
                roomNumberTo = int.Parse(ToRoom.SelectedItem.ToString().Split(" - ")[0]);
                bool messageShowed = false;

                Dictionary<int, int> equipmentQuantity = new Dictionary<int, int>();

                foreach (DistributionEquipmentReview item in reviewsFrom.DistributionEquipmentReviews)
                {
                    if (item.QuantityToMove > item.Quantity)
                    {
                        messageShowed = true;
                        MessageBox.Show("There is not enough equipment.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        if (item.QuantityToMove > 0)
                        {
                            equipmentQuantity.Add(item.Id, item.QuantityToMove);
                        }
                    }
                }

                if (equipmentQuantity.Count == 0)
                {
                    if (!messageShowed) MessageBox.Show("Please enter move quantity.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    GlobalRepository.equipmentRoomRepository.MoveEquipment(roomNumberFrom, roomNumberTo, equipmentQuantity);
                    UpdateTableViews();
                    MessageBox.Show("Saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void UpdateTableViews()
        {
            reviewsFrom = new DistributionEquipmentTable();
            reviewsTo = new DistributionEquipmentTable();

            reviewsFrom.UpdateEquipment(roomNumberFrom, true);
            reviewsTo.UpdateEquipment(roomNumberTo, true);

            FromTable.ItemsSource = reviewsFrom.DistributionEquipmentReviews;
            ToTable.ItemsSource = reviewsTo.DistributionEquipmentReviews;
        }

        private void NumericColumn(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }
}
