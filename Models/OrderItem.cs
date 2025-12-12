using System;
using System.ComponentModel;

namespace ResManager.Models
{
    public class OrderItem : INotifyPropertyChanged
    {
        private int _dishId;
        private string _dishName = string.Empty;
        private int _quantity;
        private decimal _unitPrice;
        private string _specialInstructions = string.Empty;

        public int DishId
        {
            get => _dishId;
            set
            {
                _dishId = value;
                OnPropertyChanged(nameof(DishId));
            }
        }

        public string DishName
        {
            get => _dishName;
            set
            {
                _dishName = value;
                OnPropertyChanged(nameof(DishName));
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(Subtotal));
            }
        }

        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                _unitPrice = value;
                OnPropertyChanged(nameof(UnitPrice));
                OnPropertyChanged(nameof(Subtotal));
            }
        }

        public string SpecialInstructions
        {
            get => _specialInstructions;
            set
            {
                _specialInstructions = value;
                OnPropertyChanged(nameof(SpecialInstructions));
            }
        }

        public decimal Subtotal => Quantity * UnitPrice;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{DishName} x{Quantity} - ${Subtotal:F2}";
        }
    }
}

