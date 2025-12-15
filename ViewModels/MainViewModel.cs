using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestoManager.Models;
using RestoManager.Services;
using RestoManager.Views;

namespace RestoManager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly RestaurantService _restaurantService;
        private FirestoreListenerService? _firestoreService;
        private int _ordersRefreshTrigger;
        // Take Away feature removed

        public MainViewModel()
        {
            _restaurantService = new RestaurantService();
            _restaurantService.Orders.CollectionChanged += Orders_CollectionChanged;
            SubscribeToOrderChanges();
            InitializeCommands();
        }

        private void Orders_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Order order in e.NewItems)
                {
                    order.PropertyChanged += Order_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (Order order in e.OldItems)
                {
                    order.PropertyChanged -= Order_PropertyChanged;
                }
            }

            // Notify that Orders collection changed to refresh bindings
            OnPropertyChanged(nameof(Orders));
        }

        private void Order_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Order.TotalAmount))
            {
                // Trigger binding refresh
                OrdersRefreshTrigger++;
            }
        }

        private void SubscribeToOrderChanges()
        {
            foreach (var order in _restaurantService.Orders)
            {
                order.PropertyChanged += Order_PropertyChanged;
            }
        }

        public RestaurantService Service => _restaurantService;
        
        public FirestoreListenerService? FirestoreService 
        { 
            get => _firestoreService; 
            set => _firestoreService = value; 
        }

        public ObservableCollection<Table> Tables => _restaurantService.Tables;
        public ObservableCollection<Order> Orders => _restaurantService.Orders;
        public ObservableCollection<Reservation> Reservations => _restaurantService.Reservations;

        // Property that changes when orders are modified to trigger binding updates
        public int OrdersRefreshTrigger
        {
            get => _ordersRefreshTrigger;
            private set => SetProperty(ref _ordersRefreshTrigger, value);
        }

        public ICommand ShowAllOrdersCommand { get; private set; } = null!;
        public ICommand ShowCheckoutCommand { get; private set; } = null!;
        public ICommand ShowAccountsCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            ShowAllOrdersCommand = new RelayCommand(ShowAllOrders);
            ShowCheckoutCommand = new RelayCommand<Table>(ShowCheckout);
            ShowAccountsCommand = new RelayCommand(ShowAccounts);
        }

        private void ShowAccounts()
        {
            MessageBox.Show("Accounts feature coming soon!", "Accounts");
        }


        private void ShowAllOrders()
        {
            if (FirestoreService == null)
            {
                MessageBox.Show("Firestore service is not initialized yet.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var vm = new AllOrdersViewModel(FirestoreService);
            var window = new Views.AllOrdersWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };
            window.Show();
        }

        // Take Away feature removed

        private void ShowCheckout(Table? table)
        {
            if (table == null) return;

            var checkoutWindow = new Views.CheckoutWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new CheckoutViewModel(table, _restaurantService)
            };
            checkoutWindow.ShowDialog();
        }
    }
}

