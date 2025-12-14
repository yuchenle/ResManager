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
        private Dish? _selectedDish;
        private int _ordersRefreshTrigger;
        private int _takeAwayCount = 0;

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
        public ObservableCollection<Dish> Dishes => _restaurantService.Dishes;
        public ObservableCollection<Order> Orders => _restaurantService.Orders;
        public ObservableCollection<Reservation> Reservations => _restaurantService.Reservations;

        // Property that changes when orders are modified to trigger binding updates
        public int OrdersRefreshTrigger
        {
            get => _ordersRefreshTrigger;
            private set => SetProperty(ref _ordersRefreshTrigger, value);
        }

        public Dish? SelectedDish
        {
            get => _selectedDish;
            set
            {
                SetProperty(ref _selectedDish, value);
                if (DeleteDishCommand is RelayCommand<Dish> relayCommand)
                {
                    relayCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public ICommand CreateTakeAwayCommand { get; private set; } = null!;
        public ICommand ShowMenuCommand { get; private set; } = null!;
        public ICommand ShowAllOrdersCommand { get; private set; } = null!;
        public ICommand ShowCheckoutCommand { get; private set; } = null!;
        public ICommand AddDishCommand { get; private set; } = null!;
        public ICommand DeleteDishCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            CreateTakeAwayCommand = new RelayCommand(CreateTakeAway);
            ShowMenuCommand = new RelayCommand(ShowMenu);
            ShowAllOrdersCommand = new RelayCommand(ShowAllOrders);
            ShowCheckoutCommand = new RelayCommand<Table>(ShowCheckout);
            AddDishCommand = new RelayCommand(AddDish);
            DeleteDishCommand = new RelayCommand<Dish>(DeleteDish, CanDeleteDish);
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

        private void CreateTakeAway()
        {
            var vm = new CreateTakeAwayViewModel(_restaurantService);
            var window = new Views.CreateTakeAwayWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };

            if (window.ShowDialog() == true && vm.CurrentItems.Count > 0)
            {
                var table = new Table
                {
                    Capacity = 2,
                    Location = "Take Away",
                    Status = TableStatus.Available, // AddOrder will update this to Occupied
                    Name = $"Emp_{_takeAwayCount++}"
                };

                _restaurantService.AddTable(table);

                var order = new Order
                {
                    TableId = table.Id,
                    Status = OrderStatus.Pending,
                    Notes = "Take Away"
                };

                foreach (var item in vm.CurrentItems)
                {
                    order.Items.Add(item);
                }

                _restaurantService.AddOrder(order);
            }
        }

        private void ShowMenu()
        {
            var menuWindow = new Views.MenuWindow(this)
            {
                Owner = Application.Current.MainWindow
            };
            menuWindow.Show();
        }

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

        private void AddDish()
        {
            var dialog = new Views.CreateDishDialog
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true && dialog.CreatedDish != null)
            {
                _restaurantService.AddDish(dialog.CreatedDish);
            }
        }

        private bool CanDeleteDish(Dish? dish)
        {
            return dish != null;
        }

        private void DeleteDish(Dish? dish)
        {
            if (dish == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{dish.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _restaurantService.RemoveDish(dish);
            }
        }
    }
}

