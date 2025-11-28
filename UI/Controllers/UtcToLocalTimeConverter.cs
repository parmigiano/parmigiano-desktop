using System;
using System.Globalization;
using System.Windows.Data;

namespace Parmigiano.UI.Controllers
{
    public class UtcToLocalTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";

            if (value is DateTime dt)
            {
                return dt.AddHours(5).ToString("HH:mm");
            }

            if (value is string s && DateTime.TryParse(s, out var parsed))
            {
                return parsed.AddHours(5).ToString("HH:mm");
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
