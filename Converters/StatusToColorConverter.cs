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
                    TableStatus.Available => new SolidColorBrush(Colors.LightGreen),
                    TableStatus.Occupied => new SolidColorBrush(Colors.LightCoral),
                    TableStatus.Reserved => new SolidColorBrush(Colors.LightYellow),
                    TableStatus.Cleaning => new SolidColorBrush(Colors.LightGray),
                    _ => new SolidColorBrush(Colors.White)
                };
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

