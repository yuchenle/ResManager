using System;
using System.Globalization;
using System.Windows.Data;
using RestoManager.Services;

namespace RestoManager.Converters
{
    public class LanguageNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string languageCode)
            {
                return languageCode switch
                {
                    "en" => LocalizationService.Instance.GetString("English"),
                    "fr" => LocalizationService.Instance.GetString("French"),
                    "zh" => LocalizationService.Instance.GetString("Chinese"),
                    _ => languageCode
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
