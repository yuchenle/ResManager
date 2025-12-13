using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ResManager.Models;
using ResManager.Services;

namespace ResManager.ViewModels
{
    public class TableDetailsViewModel : ObservableObject
    {
        private readonly Table _table;
        private readonly RestaurantService _restaurantService;
        private Dish? _selectedDish;
        private Order? _currentOrder;
        private OrderItem? _selectedOrderItem;

        public TableDetailsViewModel(Table table, RestaurantService restaurantService)
        {
            _table = table;
            _restaurantService = restaurantService;
            AllOrderItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalPrice));
            InitializeCommands();
            LoadTableDetails();
        }

        public int TableId => _table.Id;
        public string Location => _table.Location;
        public int Capacity => _table.Capacity;
        public TableStatus Status => _table.Status;

        public ObservableCollection<OrderItem> AllOrderItems { get; } = new();
        public ObservableCollection<Dish> AvailableDishes => _restaurantService.Dishes;
        
        public Dish? SelectedDish
        {
            get => _selectedDish;
            set
            {
                SetProperty(ref _selectedDish, value);
                if (AddDishToTableCommand is RelayCommand<Dish> relayCommand)
                {
                    relayCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public OrderItem? SelectedOrderItem
        {
            get => _selectedOrderItem;
            set
            {
                SetProperty(ref _selectedOrderItem, value);
                if (DeleteOrderItemCommand is RelayCommand<OrderItem> relayCommand)
                {
                    relayCommand.NotifyCanExecuteChanged();
                }
                if (IncreaseQuantityCommand is RelayCommand<OrderItem> increaseCommand)
                {
                    increaseCommand.NotifyCanExecuteChanged();
                }
                if (DecreaseQuantityCommand is RelayCommand<OrderItem> decreaseCommand)
                {
                    decreaseCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public decimal TotalPrice => AllOrderItems.Sum(item => item.Subtotal);

        public ICommand AddDishToTableCommand { get; private set; } = null!;
        public ICommand DeleteOrderItemCommand { get; private set; } = null!;
        public ICommand IncreaseQuantityCommand { get; private set; } = null!;
        public ICommand DecreaseQuantityCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            AddDishToTableCommand = new RelayCommand<Dish>(AddDishToTable, CanAddDishToTable);
            DeleteOrderItemCommand = new RelayCommand<OrderItem>(DeleteOrderItem, CanDeleteOrderItem);
            IncreaseQuantityCommand = new RelayCommand<OrderItem>(IncreaseQuantity, CanModifyQuantity);
            DecreaseQuantityCommand = new RelayCommand<OrderItem>(DecreaseQuantity, CanModifyQuantity);
        }

        private bool CanAddDishToTable(Dish? dish)
        {
            return dish != null && dish.IsAvailable;
        }

        private void AddDishToTable(Dish? dish)
        {
            if (dish == null || !dish.IsAvailable) return;

            // Get or create current order for this table
            if (_currentOrder == null)
            {
                _currentOrder = _restaurantService.Orders
                    .FirstOrDefault(o => o.TableId == _table.Id && o.Status != OrderStatus.Paid && o.Status != OrderStatus.Cancelled);
                
                if (_currentOrder == null)
                {
                    // Create new order
                    _currentOrder = new Order
                    {
                        TableId = _table.Id,
                        Status = OrderStatus.Pending,
                        Notes = string.Empty
                    };
                    _restaurantService.AddOrder(_currentOrder);
                }
            }

            // Check if dish already exists in order
            var existingItem = _currentOrder.Items.FirstOrDefault(i => i.DishId == dish.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                _currentOrder.Items.Add(new OrderItem
                {
                    DishId = dish.Id,
                    DishName = dish.Name,
                    UnitPrice = dish.Price,
                    Quantity = 1
                });
            }

            // Refresh the display
            LoadTableDetails();
            
            // Update table status if needed
            if (_table.Status == TableStatus.Available)
            {
                _restaurantService.UpdateTableStatus(_table.Id, TableStatus.Occupied);
            }
        }

        private void LoadTableDetails()
        {
            AllOrderItems.Clear();
            
            // Get all orders for this table (including paid ones to show complete history)
            var allOrders = _restaurantService.Orders.Where(o => o.TableId == _table.Id).ToList();
            
            foreach (var order in allOrders)
            {
                foreach (var item in order.Items)
                {
                    // Create a copy of the order item to avoid reference issues
                    AllOrderItems.Add(new OrderItem
                    {
                        DishId = item.DishId,
                        DishName = item.DishName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        SpecialInstructions = item.SpecialInstructions
                    });
                }
            }

            // Update current order reference
            _currentOrder = allOrders
                .FirstOrDefault(o => o.Status != OrderStatus.Paid && o.Status != OrderStatus.Cancelled);
        }

        private bool CanDeleteOrderItem(OrderItem? item)
        {
            // Only allow deletion if there's a current active order and an item is selected
            if (item == null || _currentOrder == null) return false;
            
            // Check if the item belongs to the current active order
            return _currentOrder.Items.Any(i => 
                i.DishId == item.DishId && 
                i.DishName == item.DishName &&
                i.UnitPrice == item.UnitPrice);
        }

        private void DeleteOrderItem(OrderItem? item)
        {
            if (item == null || _currentOrder == null) return;

            // Only allow deletion from the current active order
            // Find the corresponding item in the actual order by matching key properties
            var orderItem = _currentOrder.Items.FirstOrDefault(i => 
                i.DishId == item.DishId && 
                i.DishName == item.DishName &&
                i.UnitPrice == item.UnitPrice);

            if (orderItem != null)
            {
                _currentOrder.Items.Remove(orderItem);
                
                // Refresh the display
                LoadTableDetails();
                
                // Update table status if order is empty
                if (_currentOrder != null && _currentOrder.Items.Count == 0 && _table.Status == TableStatus.Occupied)
                {
                    _restaurantService.UpdateTableStatus(_table.Id, TableStatus.Available);
                }
            }
        }

        private bool CanModifyQuantity(OrderItem? item)
        {
            // Only allow modification if there's a current active order and an item is provided
            if (item == null || _currentOrder == null) return false;
            
            // Check if the item belongs to the current active order
            return _currentOrder.Items.Any(i => 
                i.DishId == item.DishId && 
                i.DishName == item.DishName &&
                i.UnitPrice == item.UnitPrice);
        }

        private void IncreaseQuantity(OrderItem? item)
        {
            if (item == null || _currentOrder == null) return;

            // Find the corresponding item in the actual order
            var orderItem = _currentOrder.Items.FirstOrDefault(i => 
                i.DishId == item.DishId && 
                i.DishName == item.DishName &&
                i.UnitPrice == item.UnitPrice);

            if (orderItem != null)
            {
                orderItem.Quantity++;
                // Refresh the display
                LoadTableDetails();
            }
        }

        private void DecreaseQuantity(OrderItem? item)
        {
            if (item == null || _currentOrder == null) return;

            // Find the corresponding item in the actual order
            var orderItem = _currentOrder.Items.FirstOrDefault(i => 
                i.DishId == item.DishId && 
                i.DishName == item.DishName &&
                i.UnitPrice == item.UnitPrice);

            if (orderItem != null && orderItem.Quantity > 1)
            {
                orderItem.Quantity--;
                // Refresh the display
                LoadTableDetails();
            }
            else if (orderItem != null && orderItem.Quantity == 1)
            {
                // If quantity is 1, remove the item instead
                _currentOrder.Items.Remove(orderItem);
                // Refresh the display
                LoadTableDetails();
                
                // Update table status if order is empty
                if (_currentOrder != null && _currentOrder.Items.Count == 0 && _table.Status == TableStatus.Occupied)
                {
                    _restaurantService.UpdateTableStatus(_table.Id, TableStatus.Available);
                }
            }
        }
    }
}
