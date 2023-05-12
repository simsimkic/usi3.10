using HealthInstitution.Class;
using HealthInstitution.Repository;
using HealthInstitution.TableClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HealthInstitution.ManagerView
{
    public partial class DistributionStaticEquipment : Window
    {
        DistributionEquipmentTable reviewsFrom = new DistributionEquipmentTable();
        DistributionEquipmentTable reviewsTo = new DistributionEquipmentTable();
        int roomNumberFrom, roomNumberTo;
        List<string> rooms = GlobalRepository.equipmentRoomRepository.GetRooms();

        public DistributionStaticEquipment()
        {
            InitializeComponent();
            FromRoom.ItemsSource = rooms;
            ToRoom.ItemsSource = rooms;

            GlobalRepository.equipmentRoomRepository.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "equipmentMoved")
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
                    reviewsFrom.UpdateEquipment(roomNumberFrom);
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
                    reviewsTo.UpdateEquipment(roomNumberTo);
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
            if (FromRoom.SelectedItem == null || ToRoom.SelectedItem == null || DatePicker.SelectedDate == null || DatePicker.SelectedDate.Value.Date <= DateTime.Now)
                MessageBox.Show("Please select rooms and date.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                bool messageShowed = false;
                DistributionRepository distributionRepository = new DistributionRepository();
                Dictionary<int, int> equipmentQuantity = new Dictionary<int, int>();

                foreach (DistributionEquipmentReview item in reviewsFrom.DistributionEquipmentReviews)
                {
                    if (item.QuantityToMove > item.Quantity - item.ReservedQuantity)
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
                    Distribution distribution = new Distribution(roomNumberFrom, roomNumberTo, equipmentQuantity, DatePicker.SelectedDate.Value.Date);
                    distributionRepository.Add(distribution);
                    distributionRepository.Save();
                    UpdateTableViews(roomNumberFrom, roomNumberTo);
                    MessageBox.Show("Saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void UpdateTableViews(int roomNumberFrom, int roomNumberTo)
        {
            reviewsFrom = new DistributionEquipmentTable();
            reviewsTo = new DistributionEquipmentTable();

            reviewsFrom.UpdateEquipment(roomNumberFrom);
            reviewsTo.UpdateEquipment(roomNumberTo);

            FromTable.ItemsSource = reviewsFrom.DistributionEquipmentReviews;
            ToTable.ItemsSource = reviewsTo.DistributionEquipmentReviews;
        }

        private void NumericColumn(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

    }
}
