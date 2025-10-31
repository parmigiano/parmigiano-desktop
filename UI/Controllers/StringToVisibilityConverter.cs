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
    public class StringToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNullOrEmpty = string.IsNullOrWhiteSpace(value as string);
            if (Inverse)
            {
                return isNullOrEmpty ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return isNullOrEmpty ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
