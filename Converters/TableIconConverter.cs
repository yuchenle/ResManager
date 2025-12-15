using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using RestoManager.Models;

namespace RestoManager.Converters
{
    public class TableIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string iconPath = null;

            if (value is Table table)
            {
                // Check for online orders (Web_ prefix)
                if (table.Name.StartsWith("Web_", StringComparison.OrdinalIgnoreCase))
                {
                    iconPath = "pack://application:,,,/Assets/icons/web.ico";
                }
                // Regular tables based on capacity
                else if (table.Capacity <= 2)
                {
                    iconPath = "pack://application:,,,/Assets/1_2.ico";
                }
                else
                {
                    iconPath = "pack://application:,,,/Assets/2Plus.ico";
                }
            }
            else if (value is int capacity)
            {
                // Fallback for backward compatibility if capacity is passed directly
                if (capacity <= 2)
                {
                    iconPath = "pack://application:,,,/Assets/1_2.ico";
                }
                else
                {
                    iconPath = "pack://application:,,,/Assets/2Plus.ico";
                }
            }

            if (iconPath != null)
            {
                try 
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
                    bitmap.EndInit();
                    return bitmap;
                }
                catch
                {
                    // Fallback or swallow exception if resource not found
                    return null;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
