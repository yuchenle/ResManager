using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using RestoManager.Models;

namespace RestoManager.Converters
{
    public class TableTotalPriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return "Total: $0.00";
            
            // values[0] = Table, values[1] = Orders collection, values[2] = RefreshTrigger (optional)
            if (values[0] is Table table && values[1] is ObservableCollection<Order> orders)
            {
                // Sum all active (non-paid, non-cancelled) orders for this table
                var total = orders
                    .Where(o => o.TableId == table.Id && 
                                o.Status != OrderStatus.Paid && 
                                o.Status != OrderStatus.Cancelled)
                    .Sum(o => o.TotalAmount);
                
                return $"Total: ${total:F2}";
            }
            
            return "Total: $0.00";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
