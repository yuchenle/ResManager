using System;
using System.Globalization;
using System.Windows.Data;
using ResManager.Models;

namespace ResManager.Converters
{
    public class TableEqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return false;

            var currentTable = values[0] as Table;
            var selectedTable = values[1] as Table;

            if (currentTable == null || selectedTable == null)
                return false;

            return currentTable.Id == selectedTable.Id;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

