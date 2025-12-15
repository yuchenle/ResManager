using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestoManager.Models;
using RestoManager.Services;

namespace RestoManager.ViewModels
{
    public class CreateTakeAwayViewModel : ObservableObject
    {
        private readonly RestaurantService _restaurantService;
        private OrderItem? _selectedOrderItem;
        private string _newItemName = string.Empty;
        private decimal _newItemPrice;
        private int _newItemQuantity = 1;

        public CreateTakeAwayViewModel(RestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
            CurrentItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalPrice));
            InitializeCommands();
        }

        public ObservableCollection<OrderItem> CurrentItems { get; } = new();

        public string NewItemName
        {
            get => _newItemName;
            set
            {
                SetProperty(ref _newItemName, value);
                ((RelayCommand)AddItemCommand).NotifyCanExecuteChanged();
            }
        }

        public decimal NewItemPrice
        {
            get => _newItemPrice;
            set => SetProperty(ref _newItemPrice, value);
        }

        public int NewItemQuantity
        {
            get => _newItemQuantity;
            set => SetProperty(ref _newItemQuantity, value);
        }

        public OrderItem? SelectedOrderItem
        {
            get => _selectedOrderItem;
            set
            {
                SetProperty(ref _selectedOrderItem, value);
                if (RemoveItemCommand is RelayCommand<OrderItem> removeCommand)
                {
                    removeCommand.NotifyCanExecuteChanged();
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

        public decimal TotalPrice => CurrentItems.Sum(item => item.Subtotal);

        public ICommand AddItemCommand { get; private set; } = null!;
        public ICommand RemoveItemCommand { get; private set; } = null!;
        public ICommand IncreaseQuantityCommand { get; private set; } = null!;
        public ICommand DecreaseQuantityCommand { get; private set; } = null!;
        public ICommand ConfirmCommand { get; private set; } = null!;
        public ICommand CancelCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            AddItemCommand = new RelayCommand(AddItem, CanAddItem);
            RemoveItemCommand = new RelayCommand<OrderItem>(RemoveItem, CanModifyItem);
            IncreaseQuantityCommand = new RelayCommand<OrderItem>(IncreaseQuantity, CanModifyItem);
            DecreaseQuantityCommand = new RelayCommand<OrderItem>(DecreaseQuantity, CanModifyItem);
            ConfirmCommand = new RelayCommand<Window>(Confirm);
            CancelCommand = new RelayCommand<Window>(Cancel);
        }

        private bool CanAddItem()
        {
            return !string.IsNullOrWhiteSpace(NewItemName) && NewItemQuantity > 0;
        }

        private void AddItem()
        {
            var existingItem = CurrentItems.FirstOrDefault(i => i.DishName.Equals(NewItemName, System.StringComparison.OrdinalIgnoreCase) && i.UnitPrice == NewItemPrice);
            if (existingItem != null)
            {
                existingItem.Quantity += NewItemQuantity;
            }
            else
            {
                CurrentItems.Add(new OrderItem
                {
                    DishId = 0, // No catalog
                    DishName = NewItemName,
                    UnitPrice = NewItemPrice,
                    Quantity = NewItemQuantity
                });
            }
            
            // Reset input
            NewItemName = string.Empty;
            NewItemQuantity = 1;
            // Keep price as it might be convenient
            
            OnPropertyChanged(nameof(TotalPrice));
        }

        private bool CanModifyItem(OrderItem? item)
        {
            return item != null;
        }

        private void RemoveItem(OrderItem? item)
        {
            if (item != null)
            {
                CurrentItems.Remove(item);
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private void IncreaseQuantity(OrderItem? item)
        {
            if (item != null)
            {
                item.Quantity++;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private void DecreaseQuantity(OrderItem? item)
        {
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
                else
                {
                    CurrentItems.Remove(item);
                }
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private void Confirm(Window? window)
        {
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void Cancel(Window? window)
        {
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}
