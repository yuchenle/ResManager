using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ResManager.Models;
using ResManager.Services;
using ResManager.Views;

namespace ResManager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly RestaurantService _restaurantService;
        private Table? _selectedTable;
        private Dish? _selectedDish;
        private Order? _selectedOrder;
        private ObservableCollection<OrderItem> _currentOrderItems = new();

        public MainViewModel()
        {
            _restaurantService = new RestaurantService();
            InitializeCommands();
        }

        public ObservableCollection<Table> Tables => _restaurantService.Tables;
        public ObservableCollection<Dish> Dishes => _restaurantService.Dishes;
        public ObservableCollection<Order> Orders => _restaurantService.Orders;
        public ObservableCollection<Reservation> Reservations => _restaurantService.Reservations;

        public Table? SelectedTable
        {
            get => _selectedTable;
            set
            {
                SetProperty(ref _selectedTable, value);
                LoadTableOrders();
            }
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

        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        public ObservableCollection<OrderItem> CurrentOrderItems
        {
            get => _currentOrderItems;
            set => SetProperty(ref _currentOrderItems, value);
        }


        public ICommand AddDishToOrderCommand { get; private set; } = null!;
        public ICommand RemoveItemFromOrderCommand { get; private set; } = null!;
        public ICommand CreateOrderCommand { get; private set; } = null!;
        public ICommand UpdateOrderStatusCommand { get; private set; } = null!;
        public ICommand ClearOrderCommand { get; private set; } = null!;
        public ICommand CreateTableCommand { get; private set; } = null!;
        public ICommand ShowMenuCommand { get; private set; } = null!;
        public ICommand ShowTableDetailsCommand { get; private set; } = null!;
        public ICommand AddDishCommand { get; private set; } = null!;
        public ICommand DeleteDishCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            AddDishToOrderCommand = new RelayCommand<Dish>(AddDishToOrder);
            RemoveItemFromOrderCommand = new RelayCommand<OrderItem>(RemoveItemFromOrder);
            CreateOrderCommand = new RelayCommand(CreateOrder, CanCreateOrder);
            UpdateOrderStatusCommand = new RelayCommand<OrderStatus>(UpdateOrderStatus);
            ClearOrderCommand = new RelayCommand(ClearOrder);
            CreateTableCommand = new RelayCommand(CreateTable);
            ShowMenuCommand = new RelayCommand(ShowMenu);
            ShowTableDetailsCommand = new RelayCommand<Table>(ShowTableDetails);
            AddDishCommand = new RelayCommand(AddDish);
            DeleteDishCommand = new RelayCommand<Dish>(DeleteDish, CanDeleteDish);
        }

        private void AddDishToOrder(Dish? dish)
        {
            if (dish == null || !dish.IsAvailable) return;

            var existingItem = CurrentOrderItems.FirstOrDefault(i => i.DishId == dish.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                CurrentOrderItems.Add(new OrderItem
                {
                    DishId = dish.Id,
                    DishName = dish.Name,
                    UnitPrice = dish.Price,
                    Quantity = 1
                });
            }
        }

        private void RemoveItemFromOrder(OrderItem? item)
        {
            if (item != null)
            {
                CurrentOrderItems.Remove(item);
            }
        }

        private bool CanCreateOrder()
        {
            return SelectedTable != null && CurrentOrderItems.Count > 0;
        }

        private void CreateOrder()
        {
            if (SelectedTable == null || CurrentOrderItems.Count == 0) return;

            var order = new Order
            {
                TableId = SelectedTable.Id,
                Status = OrderStatus.Pending,
                Notes = string.Empty
            };

            foreach (var item in CurrentOrderItems)
            {
                order.Items.Add(new OrderItem
                {
                    DishId = item.DishId,
                    DishName = item.DishName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    SpecialInstructions = item.SpecialInstructions
                });
            }

            _restaurantService.AddOrder(order);
            ClearOrder();
        }

        private void UpdateOrderStatus(OrderStatus status)
        {
            if (SelectedOrder != null)
            {
                _restaurantService.UpdateOrderStatus(SelectedOrder.Id, status);
            }
        }

        private void ClearOrder()
        {
            CurrentOrderItems.Clear();
            SelectedTable = null;
        }

        private void LoadTableOrders()
        {
            if (SelectedTable != null)
            {
                var orders = _restaurantService.GetOrdersByTable(SelectedTable.Id);
                // You can bind this to a separate collection if needed
            }
        }

        private void CreateTable()
        {
            var dialog = new CreateTableDialog
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true && dialog.CreatedTable != null)
            {
                _restaurantService.AddTable(dialog.CreatedTable);
                // The ObservableCollection will automatically notify the UI
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

        private void ShowTableDetails(Table? table)
        {
            if (table == null) return;

            var detailsWindow = new Views.TableDetailsWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new TableDetailsViewModel(table, _restaurantService)
            };
            detailsWindow.ShowDialog();
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

