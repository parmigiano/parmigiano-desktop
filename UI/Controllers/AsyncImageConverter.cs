using Parmigiano.Utilities;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Parmigiano.UI.Controllers
{
    public class AsyncImageConverter : IValueConverter
    {
        private readonly ImageUtilities _imageUtilities = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? url = value as string;

            var brush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Public/assets/loading-fallback-img.png")),
                Stretch = Stretch.UniformToFill
            };

            _ = System.Threading.Tasks.Task.Run(() =>
            {
                this._imageUtilities.LoadImageAsync(url, brush);
            });

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
