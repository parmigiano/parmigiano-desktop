using System;
using System.Globalization;
using System.Windows.Data;

namespace Parmigiano.UI.Controllers
{
    public class LastOnlineMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return "был(а) недавно";

            var lastOnlineObj = values[0];
            var isOnlineObj = values[1];

            bool isOnline = isOnlineObj is bool b && b;

            if (isOnline)
                return "в сети";

            if (lastOnlineObj == null || lastOnlineObj == DBNull.Value)
                return "был(а) давно";

            if (lastOnlineObj is not DateTime lastOnline)
                return "был(а) недавно";

            if (lastOnline.Kind == DateTimeKind.Unspecified)
                lastOnline = DateTime.SpecifyKind(lastOnline, DateTimeKind.Local).ToUniversalTime();
            else if (lastOnline.Kind == DateTimeKind.Local)
                lastOnline = lastOnline.ToUniversalTime();

            var diff = DateTime.UtcNow - lastOnline;

            if (diff.TotalSeconds < 0)
                diff = TimeSpan.Zero;

            if (diff.TotalSeconds < 60)
                return "был(а) только что";

            if (diff.TotalMinutes < 60)
                return $"был(а) {Math.Floor(diff.TotalMinutes)} мин. назад";

            if (diff.TotalHours < 24)
                return $"был(а) {Math.Floor(diff.TotalHours)} ч. назад";

            if (diff.TotalDays < 2)
                return "был(а) вчера";

            if (diff.TotalDays < 7)
                return $"был(а) {Math.Floor(diff.TotalDays)} дн. назад";

            if (diff.TotalDays < 30)
                return $"был(а) {Math.Floor(diff.TotalDays / 7)} нед. назад";

            return $"был(а) {Math.Floor(diff.TotalDays / 30)} мес. назад";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
