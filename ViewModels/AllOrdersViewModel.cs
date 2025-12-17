using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RestoManager.Models;
using RestoManager.Resources;
using RestoManager.Services;

namespace RestoManager.ViewModels
{
    public class AllOrdersViewModel : INotifyPropertyChanged
    {
        private readonly FirestoreListenerService _firestoreService;
        private ObservableCollection<Order> _orders;
        private IList _selectedOrders = new List<Order>();
        private bool _hasSelectedOrders;
        private LocalizedStrings _localizedStrings = LocalizedStrings.Instance;

        public AllOrdersViewModel(FirestoreListenerService firestoreService)
        {
            _firestoreService = firestoreService;
            Orders = new ObservableCollection<Order>();
            
            _firestoreService.NewOrderReceived += OnNewOrderReceived;
            DeleteOrdersCommand = new RelayCommand(DeleteOrders, () => HasSelectedOrders);
            ShowOrderDetailsCommand = new RelayCommand<Order>(ShowOrderDetails);
            LoadOrdersAsync();
        }

        private void ShowOrderDetails(Order order)
        {
            if (order == null) return;
            
            var window = new RestoManager.Views.OrderDetailsWindow(order);
            window.ShowDialog();
        }

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
            }
        }

        public IList SelectedOrders
        {
            get => _selectedOrders;
            set
            {
                _selectedOrders = value ?? new List<Order>();
                UpdateHasSelectedOrders();
            }
        }

        public void UpdateSelectedOrders(IEnumerable<Order> orders)
        {
            var orderList = new List<Order>();
            if (orders != null)
            {
                orderList.AddRange(orders);
            }
            SelectedOrders = orderList;
        }

        public bool HasSelectedOrders
        {
            get => _hasSelectedOrders;
            private set
            {
                if (_hasSelectedOrders != value)
                {
                    _hasSelectedOrders = value;
                    OnPropertyChanged(nameof(HasSelectedOrders));
                    if (DeleteOrdersCommand is RelayCommand command)
                    {
                        command.NotifyCanExecuteChanged();
                    }
                }
            }
        }

        public ICommand DeleteOrdersCommand { get; }
        public ICommand ShowOrderDetailsCommand { get; }

        private void UpdateHasSelectedOrders()
        {
            HasSelectedOrders = _selectedOrders != null && _selectedOrders.Count > 0;
        }

        private async void LoadOrdersAsync()
        {
            var orders = await _firestoreService.GetAllOrdersAsync();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }
        }

        private void OnNewOrderReceived(Order order)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Orders.Insert(0, order);
            });
        }

        private async void DeleteOrders()
        {
            if (_selectedOrders == null || _selectedOrders.Count == 0)
                return;

            var ordersToDelete = _selectedOrders.Cast<Order>().ToList();
            var documentIds = ordersToDelete
                .Where(o => !string.IsNullOrEmpty(o.FirestoreDocumentId))
                .Select(o => o.FirestoreDocumentId)
                .ToList();

            if (documentIds.Count == 0)
            {
                MessageBox.Show(
                    _localizedStrings.NoValidOrdersSelected, 
                    _localizedStrings.DeleteOrders, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
                return;
            }

            // Show confirmation dialog
            var message = documentIds.Count == 1
                ? _localizedStrings.AreYouSureDeleteOrder
                : string.Format(_localizedStrings.AreYouSureDeleteOrders, documentIds.Count);

            var result = MessageBox.Show(
                message,
                _localizedStrings.ConfirmDelete,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _firestoreService.DeleteOrdersAsync(documentIds);
                if (success)
                {
                    // Remove deleted orders from the local collection
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var order in ordersToDelete)
                        {
                            Orders.Remove(order);
                        }
                        _selectedOrders.Clear();
                        UpdateHasSelectedOrders();
                    });

                    var successMessage = documentIds.Count == 1
                        ? _localizedStrings.OrderDeletedSuccessfully
                        : string.Format(_localizedStrings.OrdersDeletedSuccessfully, documentIds.Count);

                    MessageBox.Show(
                        successMessage,
                        _localizedStrings.DeleteOrders,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
