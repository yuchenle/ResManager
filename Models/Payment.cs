using System;
using System.ComponentModel;

namespace ResManager.Models
{
    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard,
        MobilePayment
    }

    public class Payment : INotifyPropertyChanged
    {
        private int _id;
        private int _orderId;
        private decimal _amount;
        private PaymentMethod _method;
        private DateTime _paymentTime;
        private string _transactionId = string.Empty;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public int OrderId
        {
            get => _orderId;
            set
            {
                _orderId = value;
                OnPropertyChanged(nameof(OrderId));
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }

        public PaymentMethod Method
        {
            get => _method;
            set
            {
                _method = value;
                OnPropertyChanged(nameof(Method));
            }
        }

        public DateTime PaymentTime
        {
            get => _paymentTime;
            set
            {
                _paymentTime = value;
                OnPropertyChanged(nameof(PaymentTime));
            }
        }

        public string TransactionId
        {
            get => _transactionId;
            set
            {
                _transactionId = value;
                OnPropertyChanged(nameof(TransactionId));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"Payment #{Id} - Order {OrderId} - {Method} - ${Amount:F2}";
        }
    }
}

