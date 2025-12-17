using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using RestoManager.Models;
using RestoManager.ViewModels;

namespace RestoManager.Views
{
    public partial class AllOrdersWindow : Window
    {
        public AllOrdersWindow()
        {
            InitializeComponent();
        }

        private void OrdersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is AllOrdersViewModel viewModel)
            {
                var listView = sender as ListView;
                if (listView != null)
                {
                    var selectedOrders = new List<Order>();
                    foreach (var item in listView.SelectedItems)
                    {
                        // Handle grouped items - extract Order from CollectionViewGroup
                        if (item is CollectionViewGroup group)
                        {
                            foreach (Order order in group.Items)
                            {
                                selectedOrders.Add(order);
                            }
                        }
                        else if (item is Order order)
                        {
                            selectedOrders.Add(order);
                        }
                    }
                    viewModel.UpdateSelectedOrders(selectedOrders);
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
