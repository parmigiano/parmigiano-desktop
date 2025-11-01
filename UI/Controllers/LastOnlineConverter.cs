using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Parmigiano.UI.Controllers
{
    public class LastOnlineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "был(а) давно";

            if (value is not DateTime lastOnline)
                return "был(а) недавно";

            lastOnline = DateTime.SpecifyKind(lastOnline, DateTimeKind.Utc);

            var now = DateTime.UtcNow;
            var diff = now - lastOnline;

            if (diff.TotalSeconds < 60)
                return "был(а) только что";

            if (diff.TotalMinutes < 60)
                return $"был(а) {Math.Floor(diff.TotalMinutes)} минут назад";

            if (diff.TotalHours < 24)
                return $"был(а) {Math.Floor(diff.TotalHours)} часов назад";

            if (diff.TotalDays < 2)
                return "был(а) вчера";

            if (diff.TotalDays < 7)
                return $"был(а) {Math.Floor(diff.TotalDays)} дней назад";

            if (diff.TotalDays < 30)
                return $"был(а) {Math.Floor(diff.TotalDays / 7)} нед. назад";

            return $"был(а) {Math.Floor(diff.TotalDays / 30)} мес. назад";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
