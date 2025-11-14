using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Parmigiano.UI.Controllers
{
    public class PhoneVisibleMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values == null || values.Length < 2) return Visibility.Collapsed;

                bool visible = false;
                if (values[0] is bool b) visible = b;
                else if (values[0] is bool?) visible = (values[0] as bool?) == true;

                var phone = values[1] as string;
                if (!visible) return Visibility.Collapsed;

                return string.IsNullOrWhiteSpace(phone) ? Visibility.Collapsed : Visibility.Visible;
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
