using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ResManager.Converters
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
                    // Try both .ico and .icon extensions
                    string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "1_2");
                    iconPath = basePath + ".ico";
                    if (!File.Exists(iconPath))
                    {
                        iconPath = basePath + ".icon";
                    }
                }
                else
                {
                    iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "2Plus.ico");
                }

                if (File.Exists(iconPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
                    bitmap.EndInit();
                    return bitmap;
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
