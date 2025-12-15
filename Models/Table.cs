using System;
using System.ComponentModel;

namespace RestoManager.Models
{
    public enum TableStatus
    {
        Available,
        Occupied,
        Reserved,
        Cleaning
    }

    public class Table : INotifyPropertyChanged
    {
        private int _id;
        private int _capacity;
        private TableStatus _status;
        private string _location = string.Empty;
        private string _name = string.Empty;
        private bool _isBillPrinted;
        private string _clientName = string.Empty;
        private string _phoneNumber = string.Empty;
        private DateTime? _pickupTime;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                _capacity = value;
                OnPropertyChanged(nameof(Capacity));
            }
        }

        public TableStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        public bool IsBillPrinted
        {
            get => _isBillPrinted;
            set
            {
                _isBillPrinted = value;
                OnPropertyChanged(nameof(IsBillPrinted));
            }
        }

        // Web-order metadata (empty for in-house tables)
        public string ClientName
        {
            get => _clientName;
            set
            {
                _clientName = value ?? string.Empty;
                OnPropertyChanged(nameof(ClientName));
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value ?? string.Empty;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        public DateTime? PickupTime
        {
            get => _pickupTime;
            set
            {
                _pickupTime = value;
                OnPropertyChanged(nameof(PickupTime));
                OnPropertyChanged(nameof(PickupTimeDisplay));
            }
        }

        // Convenience for WPF bindings + easy hide-if-empty in XAML
        public string PickupTimeDisplay => PickupTime.HasValue ? PickupTime.Value.ToString("HH:mm") : string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Name} - {Location} ({Capacity} seats) - {Status}";
        }
    }
}

