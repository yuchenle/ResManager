using System;
using System.Globalization;
using System.Windows.Data;

namespace RestoManager.Converters
{
    public class IsWebOrderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string name)
            {
                return name.StartsWith("Web_", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
