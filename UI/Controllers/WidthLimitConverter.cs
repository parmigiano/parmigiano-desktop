using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Parmigiano.UI.Controllers
{
    public class WidthLimitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double parentWidth)
            {
                double factor = 0.95;
                if (parameter != null && double.TryParse(parameter.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
                {
                    factor = parsed;
                }

                double maxWidth = parentWidth * factor;
                return Math.Max(100, maxWidth - 20);
            }

            return 500.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
