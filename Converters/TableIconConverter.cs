using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RestoManager.Converters
{
    public class TableIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int capacity)
            {
                string iconPath;
                if (capacity <= 2)
                {
                    iconPath = "pack://application:,,,/Assets/1_2.ico";
                }
                else
                {
                    iconPath = "pack://application:,,,/Assets/2Plus.ico";
                }

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
