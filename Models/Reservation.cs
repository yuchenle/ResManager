using System;
using System.ComponentModel;

namespace RestoManager.Models
{
    public class Reservation : INotifyPropertyChanged
    {
        private int _id;
        private int _tableId;
        private string _customerName = string.Empty;
        private string _customerPhone = string.Empty;
        private string _customerEmail = string.Empty;
        private DateTime _reservationTime;
        private int _numberOfGuests;
        private string _specialRequests = string.Empty;
        private bool _isConfirmed;

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

        public string CustomerName
        {
            get => _customerName;
            set
            {
                _customerName = value;
                OnPropertyChanged(nameof(CustomerName));
            }
        }

        public string CustomerPhone
        {
            get => _customerPhone;
            set
            {
                _customerPhone = value;
                OnPropertyChanged(nameof(CustomerPhone));
            }
        }

        public string CustomerEmail
        {
            get => _customerEmail;
            set
            {
                _customerEmail = value;
                OnPropertyChanged(nameof(CustomerEmail));
            }
        }

        public DateTime ReservationTime
        {
            get => _reservationTime;
            set
            {
                _reservationTime = value;
                OnPropertyChanged(nameof(ReservationTime));
            }
        }

        public int NumberOfGuests
        {
            get => _numberOfGuests;
            set
            {
                _numberOfGuests = value;
                OnPropertyChanged(nameof(NumberOfGuests));
            }
        }

        public string SpecialRequests
        {
            get => _specialRequests;
            set
            {
                _specialRequests = value;
                OnPropertyChanged(nameof(SpecialRequests));
            }
        }

        public bool IsConfirmed
        {
            get => _isConfirmed;
            set
            {
                _isConfirmed = value;
                OnPropertyChanged(nameof(IsConfirmed));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"Reservation #{Id} - {CustomerName} - Table {TableId} - {ReservationTime:g}";
        }
    }
}

