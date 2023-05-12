using HealthInstitution.Class;
using HealthInstitution.Repository;
using HealthInstitution.TableClass;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HealthInstitution.ManagerView
{
    public partial class EquipmentReview : Window
    {
        EquipmentReviewTable equipmentReviewTable = new EquipmentReviewTable();
        List<ObservableCollection<EquipmentReviewFilterObject>> filterOptions = new List<ObservableCollection<EquipmentReviewFilterObject>>();
        int currentFilter;

        public EquipmentReview()
        {
            InitializeComponent();
            Table.ItemsSource = equipmentReviewTable.EquipmentReviews;
            InitializeFilter();
        }

        private void InitializeFilter()
        {
            InitializeEquipmentTypeFilter();
            InitializeRoomTypeFilter();
            InitializeQuantityFilter();
        }

        private void InitializeQuantityFilter()
        {
            ObservableCollection<EquipmentReviewFilterObject> temp = new ObservableCollection<EquipmentReviewFilterObject>();
            temp.Add(new EquipmentReviewFilterObject("0"));
            temp.Add(new EquipmentReviewFilterObject("10+"));
            temp.Add(new EquipmentReviewFilterObject("0-10"));
            filterOptions.Add(temp);
        }

        private void InitializeRoomTypeFilter()
        {
            ObservableCollection<EquipmentReviewFilterObject> temp = new ObservableCollection<EquipmentReviewFilterObject>();
            foreach (var el in Enum.GetValues(typeof(RoomType)))
            {
                var roomTypeDesc = el.GetType().GetMember(el.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;
                temp.Add(new EquipmentReviewFilterObject(roomTypeDesc.Description));
            }
            filterOptions.Add(temp);
        }

        private void InitializeEquipmentTypeFilter()
        {
            ObservableCollection<EquipmentReviewFilterObject> temp = new ObservableCollection<EquipmentReviewFilterObject>();
            foreach (var el in Enum.GetValues(typeof(EquipmentType)))
            {
                var equipmentTypeDesc = el.GetType().GetMember(el.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0] as DescriptionAttribute;
                temp.Add(new EquipmentReviewFilterObject(equipmentTypeDesc.Description));
            }
            filterOptions.Add(temp);
        }

        private void Search(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var filtered = equipmentReviewTable.EquipmentReviews.Where(EquipmentReview => EquipmentReview.Equipment.ToLower().Contains(SearchBox.Text.ToLower())
                                                                                       || EquipmentReview.Room.ToString().ToLower().Contains(SearchBox.Text.ToLower())
                                                                                       || EquipmentReview.EquipmentType.ToLower().Contains(SearchBox.Text.ToLower())
                                                                                       || EquipmentReview.RoomType.ToLower().Contains(SearchBox.Text.ToLower())
                                                                                       || EquipmentReview.Quantity.ToString().ToLower().Contains(SearchBox.Text.ToLower()));
            Table.ItemsSource = filtered;
        }

        private void FilterClickQuantity(object sender, RoutedEventArgs e)
        {
            currentFilter = 2;

            FilterPopup.PlacementTarget = (sender as Button);
            FilterPopup.IsOpen = true;

            WarehouseButton.Visibility = Visibility.Hidden;

            PopupItems.ItemsSource = filterOptions[currentFilter];
        }

        private void FilterClickEquipment(object sender, RoutedEventArgs e)
        {
            currentFilter = 0;

            FilterPopup.PlacementTarget = (sender as Button);
            FilterPopup.IsOpen = true;

            WarehouseButton.Visibility = Visibility.Hidden;

            PopupItems.ItemsSource = filterOptions[currentFilter];
        }

        private void FilterClickRoom(object sender, RoutedEventArgs e)
        {
            currentFilter = 1;

            FilterPopup.PlacementTarget = (sender as Button);
            FilterPopup.IsOpen = true;

            WarehouseButton.Visibility = Visibility.Visible;

            PopupItems.ItemsSource = filterOptions[currentFilter];
        }

        private void CheckAll(object sender, RoutedEventArgs e)
        {
            foreach (EquipmentReviewFilterObject el in filterOptions[currentFilter])
                el.IsChecked = true;
        }

        private void UncheckAll(object sender, RoutedEventArgs e)
        {
            foreach (EquipmentReviewFilterObject el in filterOptions[currentFilter])
                el.IsChecked = false;
        }

        private void Warehouse(object sender, RoutedEventArgs e)
        {
            foreach (EquipmentReviewFilterObject el in filterOptions[currentFilter])
            {
                if (el.Title == "Warehouse")
                    el.IsChecked = false;
                else
                    el.IsChecked = true;
            }
        }

        private bool HasEquipmentType(TableClass.EquipmentReview row)
        {
            foreach (var option in filterOptions[0])
            {
                if (option.IsChecked)
                {
                    if (row.EquipmentType.Equals(option.Title))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasRoomType(TableClass.EquipmentReview row)
        {
            foreach (var option in filterOptions[1])
            {
                if (option.IsChecked)
                {
                    if (row.RoomType.Equals(option.Title))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasQuantity(TableClass.EquipmentReview row)
        {
            foreach (var option in filterOptions[2])
            {
                if (option.IsChecked)
                {
                    if (option.Title.Equals("0"))
                    {
                        if (row.Quantity == 0) return true;
                    }
                    else if (option.Title.Equals("10+"))
                    {
                        if (row.Quantity > 10) return true;
                    }
                    else
                    {
                        if (row.Quantity <= 10 && row.Quantity > 0) return true;
                    }
                }
            }
            return false;
        }

        private void ApplyButton(object sender, RoutedEventArgs e)
        {
            ObservableCollection<TableClass.EquipmentReview> filtered = new ObservableCollection<TableClass.EquipmentReview>();

            foreach (var el in equipmentReviewTable.EquipmentReviews)
            {
                if (HasEquipmentType(el) && HasRoomType(el) && HasQuantity(el))
                    filtered.Add(el);
            }

            FilterPopup.IsOpen = false;
            Table.ItemsSource = filtered;
        }
    }
}
