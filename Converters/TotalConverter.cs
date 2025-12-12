using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using ResManager.Models;

namespace ResManager.Converters
{
    public class TotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<OrderItem> items)
            {
                var total = items.Sum(item => item.Subtotal);
                return $"Total: ${total:F2}";
            }
            return "Total: $0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

