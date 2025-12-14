using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using RestoManager.Models;
using RestoManager.Services;
using System.Windows;

namespace RestoManager.ViewModels
{
    public class AllOrdersViewModel : INotifyPropertyChanged
    {
        private readonly FirestoreListenerService _firestoreService;
        private ObservableCollection<Order> _orders;
        
        public AllOrdersViewModel(FirestoreListenerService firestoreService)
        {
            _firestoreService = firestoreService;
            Orders = new ObservableCollection<Order>();
            
            _firestoreService.NewOrderReceived += OnNewOrderReceived;
            LoadOrdersAsync();
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
