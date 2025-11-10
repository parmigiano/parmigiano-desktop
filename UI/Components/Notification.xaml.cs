using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Parmigiano.UI.Components
{
    public enum NotificationType
    {
        Success,
        Error,
        Warning
    }

    /// <summary>
    /// Логика взаимодействия для Notification.xaml
    /// </summary>
    public partial class Notification : UserControl
    {
        private static Notification? _instance;

        public Notification()
        {
            InitializeComponent();

            _instance = this;
        }

        private void ApplyType(NotificationType type)
        {
            string iconPath = "";

            switch (type)
            {
                case NotificationType.Success:
                    RootBorder.Background = new SolidColorBrush(Color.FromRgb(0x11, 0x11, 0x11));
                    RootBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));

                    iconPath = "pack://application:,,,/Public/ico/circle-check.png";
                    break;

                case NotificationType.Error:
                    RootBorder.Background = new SolidColorBrush(Color.FromRgb(0x11, 0x11, 0x11));
                    RootBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));

                    iconPath = "pack://application:,,,/Public/ico/circle-x.png";
                    break;

                case NotificationType.Warning:
                    RootBorder.Background = new SolidColorBrush(Color.FromRgb(0x11, 0x11, 0x11));
                    RootBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));

                    iconPath = "pack://application:,,,/Public/ico/info.png";
                    break;
            }

            Icon.Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
        }

        public static void Show(string title, string message, NotificationType type)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (_instance == null) return;

                _instance.TitleText.Text = title;
                _instance.MessageText.Text = message;
                _instance.ApplyType(type);

                _instance.Visibility = Visibility.Visible;

                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(100)));
                _instance.BeginAnimation(OpacityProperty, fadeIn);

                await Task.Delay(3500);

                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(50));

                fadeOut.Completed += (s, e) => _instance.Visibility = Visibility.Collapsed;
                _instance.BeginAnimation(OpacityProperty, fadeOut);
            });
        }

        public static void HideOverlay()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_instance == null) return;

                var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(50)));
                fadeOut.Completed += (s, e) =>
                {
                    _instance.Visibility = Visibility.Collapsed;
                };

                _instance.BeginAnimation(OpacityProperty, fadeOut);
            });
        }

        private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HideOverlay();
        }
    }
}
