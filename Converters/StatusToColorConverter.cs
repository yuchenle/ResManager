using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using RestoManager.Models;

namespace RestoManager.Converters
{
    public class StatusToColorConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Fallback for TableStatus directly (classic binding: {Binding Status, Converter=...})
            if (value is TableStatus status)
            {
                return ConvertStatusToColor(status);
            }
            
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F8F8"));
        }

        // MultiBinding version: {Binding Name} + {Binding Status}
        // This ensures WPF re-evaluates when either Name or Status changes.
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F8F8"));

            string name = values[0]?.ToString() ?? string.Empty;
            TableStatus status = values[1] is TableStatus s ? s : TableStatus.Available;

            if (name.StartsWith("Web_", StringComparison.OrdinalIgnoreCase))
            {
                // Web tables: keep background neutral/white
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F8F8"));
            }

            return ConvertStatusToColor(status);
        }
        
        private SolidColorBrush ConvertStatusToColor(TableStatus status)
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
