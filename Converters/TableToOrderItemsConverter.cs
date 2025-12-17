using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using RestoManager.Models;

namespace RestoManager.Converters
{
    public class TableToOrderItemsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return Enumerable.Empty<OrderItem>();
            
            // values[0] = Table, values[1] = Orders collection, values[2] = RefreshTrigger (optional)
            if (values[0] is Table table && values[1] is ObservableCollection<Order> orders)
            {
                // Get all active items for this table
                return orders
                    .Where(o => o.TableId == table.Id && 
                                o.Status != OrderStatus.Paid && 
                                o.Status != OrderStatus.Cancelled)
                    .SelectMany(o => o.Items)
                    .ToList();
            }
            
            return Enumerable.Empty<OrderItem>();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
