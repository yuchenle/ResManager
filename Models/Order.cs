using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ResManager.Models
{
    public enum OrderStatus
    {
        Pending,
        InProgress,
        Ready,
        Served,
        Paid,
        Cancelled
    }

    public class Order : INotifyPropertyChanged
    {
        private int _id;
        private int _tableId;
        private DateTime _orderTime;
        private OrderStatus _status;
        private ObservableCollection<OrderItem> _items = new();
        private string _notes = string.Empty;

        public Order()
        {
            _items.CollectionChanged += Items_CollectionChanged;
        }

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public int TableId
        {
            get => _tableId;
            set
            {
                _tableId = value;
                OnPropertyChanged(nameof(TableId));
            }
        }

        public DateTime OrderTime
        {
            get => _orderTime;
            set
            {
                _orderTime = value;
                OnPropertyChanged(nameof(OrderTime));
            }
        }

        public OrderStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public ObservableCollection<OrderItem> Items
        {
            get => _items;
            set
            {
                if (_items != null)
                {
                    _items.CollectionChanged -= Items_CollectionChanged;
                    foreach (var item in _items)
                    {
                        item.PropertyChanged -= OrderItem_PropertyChanged;
                    }
                }

                _items = value ?? new ObservableCollection<OrderItem>();
                
                if (_items != null)
                {
                    _items.CollectionChanged += Items_CollectionChanged;
                    foreach (var item in _items)
                    {
                        item.PropertyChanged += OrderItem_PropertyChanged;
                    }
                }

                OnPropertyChanged(nameof(Items));
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (OrderItem item in e.NewItems)
                {
                    item.PropertyChanged += OrderItem_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (OrderItem item in e.OldItems)
                {
                    item.PropertyChanged -= OrderItem_PropertyChanged;
                }
            }

            OnPropertyChanged(nameof(TotalAmount));
        }

        private void OrderItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderItem.Subtotal) || 
                e.PropertyName == nameof(OrderItem.Quantity) || 
                e.PropertyName == nameof(OrderItem.UnitPrice))
            {
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }

        public decimal TotalAmount => Items.Sum(item => item.Subtotal);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"Order #{Id} - Table {TableId} - {Status} - ${TotalAmount:F2}";
        }
    }
}

