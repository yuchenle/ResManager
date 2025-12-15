using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RestoManager.Converters
{
    public class CheckoutButtonColorConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // NOTE: This IValueConverter overload won't update when a property on the Table changes
            // (because the bound object reference doesn't change). Prefer the IMultiValueConverter
            // overload below via MultiBinding (Name + IsBillPrinted).
            return new SolidColorBrush(Colors.Transparent);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return new SolidColorBrush(Colors.Transparent);

            string name = values[0]?.ToString() ?? string.Empty;
            bool isPrinted = values[1] is bool b && b;

            if (name.StartsWith("Web_", StringComparison.OrdinalIgnoreCase))
            {
                // Web tables: red when not printed, green when printed
                return isPrinted ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }

            // For non-web tables, use default button style (transparent/default)
            return new SolidColorBrush(Colors.Transparent);
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

