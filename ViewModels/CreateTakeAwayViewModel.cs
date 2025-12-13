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
        private Dish? _selectedDish;
        private OrderItem? _selectedOrderItem;

        public CreateTakeAwayViewModel(RestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
            CurrentItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalPrice));
            InitializeCommands();
        }

        public ObservableCollection<OrderItem> CurrentItems { get; } = new();
        public ObservableCollection<Dish> AvailableDishes => _restaurantService.Dishes;

        public Dish? SelectedDish
        {
            get => _selectedDish;
            set
            {
                SetProperty(ref _selectedDish, value);
                if (AddDishCommand is RelayCommand<Dish> relayCommand)
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

        public ICommand AddDishCommand { get; private set; } = null!;
        public ICommand RemoveItemCommand { get; private set; } = null!;
        public ICommand IncreaseQuantityCommand { get; private set; } = null!;
        public ICommand DecreaseQuantityCommand { get; private set; } = null!;
        public ICommand ConfirmCommand { get; private set; } = null!;
        public ICommand CancelCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            AddDishCommand = new RelayCommand<Dish>(AddDish, CanAddDish);
            RemoveItemCommand = new RelayCommand<OrderItem>(RemoveItem, CanModifyItem);
            IncreaseQuantityCommand = new RelayCommand<OrderItem>(IncreaseQuantity, CanModifyItem);
            DecreaseQuantityCommand = new RelayCommand<OrderItem>(DecreaseQuantity, CanModifyItem);
            ConfirmCommand = new RelayCommand<Window>(Confirm);
            CancelCommand = new RelayCommand<Window>(Cancel);
        }

        private bool CanAddDish(Dish? dish)
        {
            return dish != null && dish.IsAvailable;
        }

        private void AddDish(Dish? dish)
        {
            if (dish == null || !dish.IsAvailable) return;

            var existingItem = CurrentItems.FirstOrDefault(i => i.DishId == dish.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                CurrentItems.Add(new OrderItem
                {
                    DishId = dish.Id,
                    DishName = dish.Name,
                    UnitPrice = dish.Price,
                    Quantity = 1
                });
            }
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
