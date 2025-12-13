using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestoManager.Models;
using RestoManager.Services;

namespace RestoManager.ViewModels
{
    public class CheckoutViewModel : ObservableObject
    {
        private readonly Table _table;
        private readonly RestaurantService _restaurantService;

        public CheckoutViewModel(Table table, RestaurantService restaurantService)
        {
            _table = table;
            _restaurantService = restaurantService;
            InitializeCommands();
            LoadBillItems();
        }

        public int TableId => _table.Id;
        public int Capacity => _table.Capacity;
        public string Location => _table.Location;

        private ObservableCollection<BillItem> _billItems = new();
        public ObservableCollection<BillItem> BillItems
        {
            get => _billItems;
            private set
            {
                if (_billItems != null)
                {
                    _billItems.CollectionChanged -= BillItems_CollectionChanged;
                }
                SetProperty(ref _billItems, value);
                if (_billItems != null)
                {
                    _billItems.CollectionChanged += BillItems_CollectionChanged;
                }
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(TaxAmount));
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        public decimal Subtotal => BillItems.Sum(item => item.Subtotal);
        public decimal TaxRate => 0.10m; // 10% tax
        public decimal TaxAmount => Subtotal * TaxRate;
        public decimal TotalAmount => Subtotal + TaxAmount;

        private void BillItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(TaxAmount));
            OnPropertyChanged(nameof(TotalAmount));
        }

        public ICommand PrintBillCommand { get; private set; } = null!;
        public ICommand CancelCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            PrintBillCommand = new RelayCommand(PrintBill);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void LoadBillItems()
        {
            BillItems.Clear();

            // Get all active (non-paid, non-cancelled) orders for this table
            var activeOrders = _restaurantService.Orders
                .Where(o => o.TableId == _table.Id && 
                            o.Status != OrderStatus.Paid && 
                            o.Status != OrderStatus.Cancelled)
                .ToList();

            foreach (var order in activeOrders)
            {
                foreach (var item in order.Items)
                {
                    BillItems.Add(new BillItem
                    {
                        DishName = item.DishName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Subtotal,
                        Tax = item.Subtotal * TaxRate
                    });
                }
            }
        }

        private void PrintBill()
        {
            // TODO: Implement print functionality
            // For now, do nothing as requested
        }

        private void Cancel()
        {
            // This will be handled by the window's DialogResult
        }
    }

    public class BillItem : ObservableObject
    {
        private string _dishName = string.Empty;
        private int _quantity;
        private decimal _unitPrice;
        private decimal _subtotal;
        private decimal _tax;

        public string DishName
        {
            get => _dishName;
            set => SetProperty(ref _dishName, value);
        }

        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public decimal UnitPrice
        {
            get => _unitPrice;
            set => SetProperty(ref _unitPrice, value);
        }

        public decimal Subtotal
        {
            get => _subtotal;
            set => SetProperty(ref _subtotal, value);
        }

        public decimal Tax
        {
            get => _tax;
            set => SetProperty(ref _tax, value);
        }
    }
}
