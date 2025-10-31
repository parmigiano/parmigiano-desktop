using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Parmigiano.UI.Controllers
{
    public class UsernameToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var username = value as string;
            if (string.IsNullOrEmpty(username))
            {
                return new SolidColorBrush(Colors.Gray);
            }

            int hash = username.GetHashCode();
            byte r = (byte)(hash & 0xFF);
            byte g = (byte)((hash >> 8) & 0xFF);
            byte b = (byte)((hash >> 16) & 0xFF);

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
