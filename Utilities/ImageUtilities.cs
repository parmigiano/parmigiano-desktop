using System;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Parmigiano.Utilities
{
    public class ImageUtilities
    {
        private static readonly Uri LoadingFallbackUri = new("pack://application:,,,/Public/assets/loading-fallback-img.png");
        private static readonly Uri ErrorFallbackUri = new("pack://application:,,,/Public/assets/error-fallback-img.jpg");

        private static BitmapImage CreateFrozenImage(Uri uri)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = uri;
            image.EndInit();
            image.Freeze();
            return image;
        }

        public async void LoadImageAsync(string url, object target)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SetImageSource(target, CreateFrozenImage(LoadingFallbackUri));
            });

            if (string.IsNullOrWhiteSpace(url))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetImageSource(target, CreateFrozenImage(ErrorFallbackUri));
                });

                return;
            }

            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(60);

                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetImageSource(target, CreateFrozenImage(ErrorFallbackUri));
                    });
                        
                    return;
                }

                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                var bitmap = new BitmapImage();
                using (var stream = new MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetImageSource(target, bitmap);
                });
            }
            catch
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetImageSource(target, CreateFrozenImage(ErrorFallbackUri));
                });
            }
        }

        private void SetImageSource(object target, ImageSource source)
        {
            switch (target)
            {
                case Image image:
                    image.Source = source;
                    break;

                case ImageBrush brush:
                    brush.ImageSource = source;
                    break;
            }
        }
    }
}
