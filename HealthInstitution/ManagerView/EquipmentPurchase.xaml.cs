using HealthInstitution.Class;
using HealthInstitution.Repository;
using HealthInstitution.TableClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HealthInstitution.ManagerView
{
    public partial class EquipmentPurchase : Window
    {
        EquipmentPurchaseTable equipmentPurchaseTable = new EquipmentPurchaseTable();
        
        public EquipmentPurchase()
        {
            InitializeComponent();
            ShowTable.ItemsSource = equipmentPurchaseTable.EquipmentPurchaseReviews;
            BuyTable.ItemsSource = equipmentPurchaseTable.EquipmentPurchaseReviews;
            GlobalRepository.equipmentRoomRepository.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "dynamicChanged" || e.PropertyName == "warehouseChanged")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    equipmentPurchaseTable = new EquipmentPurchaseTable();
                    ShowTable.ItemsSource = equipmentPurchaseTable.EquipmentPurchaseReviews;
                    BuyTable.ItemsSource = equipmentPurchaseTable.EquipmentPurchaseReviews;
                });
            }
        }

        private void NumericColumn(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void Purchase(object sender, RoutedEventArgs e)
        {
            Dictionary<int, int> items = MakeItemDictionary();
            DateTime date = DateTime.Now;

            if (items.Count > 0)
            {
                Purchase purchase = new Purchase(items, date);
                PurchaseRepository purchaseRepository = new PurchaseRepository();
                purchaseRepository.Add(purchase);
                equipmentPurchaseTable = new EquipmentPurchaseTable();
                ShowTable.ItemsSource = equipmentPurchaseTable.EquipmentPurchaseReviews;
                BuyTable.ItemsSource = equipmentPurchaseTable.EquipmentPurchaseReviews;
                MessageBox.Show("Purchase was successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Please select equipment and enter quantity.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private Dictionary<int, int> MakeItemDictionary()
        {
            Dictionary<int, int> items = new Dictionary<int, int>();
            foreach (var item in equipmentPurchaseTable.EquipmentPurchaseReviews)
            {
                if (item.Buy && item.PurchaseQuantity > 0) {
                    items.Add(item.Id, item.PurchaseQuantity);
                }
            }

            return items;
        }
    }
}
