using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using RestoManager.Models;

namespace RestoManager.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TableStatus status)
            {
                return status switch
                {
                    TableStatus.Available => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#587E76")), // Idle/Muted Teal
                    TableStatus.Occupied => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A33A3A")), // Attention/Deep Burgundy
                    TableStatus.Reserved => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDAE7F")), // Muted Gold
                    TableStatus.Cleaning => new SolidColorBrush(Colors.LightGray),
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F8F8"))
                };
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F8F8"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
