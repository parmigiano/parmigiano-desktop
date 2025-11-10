using Microsoft.Win32;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Parmigiano.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для UserProfileModal.xaml
    /// </summary>
    public partial class UserProfileModal : UserControl
    {
        private static ImageUtilities _imageUtilities = new ImageUtilities();
        private static UserProfileModal _instance;

        public UserProfileModal()
        {
            InitializeComponent();
            _instance = this;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            HideOverlay();
        }

        private void Root_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsClickInsideDialog(e))
            {
                HideOverlay();
            }
        }

        private bool IsClickInsideDialog(System.Windows.Input.MouseButtonEventArgs e)
        {
            var clickedElement = e.OriginalSource as DependencyObject;

            while (clickedElement != null)
            {
                if (clickedElement == Dialog)
                {
                    return true;
                }    
                   
                clickedElement = VisualTreeHelper.GetParent(clickedElement);
            }

            return false;
        }

        public static void ShowProfile(ChatMinimalWithLMessageModel user)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_instance == null) return;

                _instance.DataContext = user;

                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    _imageUtilities.LoadImageAsync(user.Avatar, _instance.AvatarImage);
                    _instance.InitialText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _instance.AvatarImage.ImageSource = null;
                    _instance.InitialText.Visibility = Visibility.Visible;

                    if (!string.IsNullOrEmpty(user.Username))
                    {
                        _instance.InitialText.Text = user.Username.Substring(0, 1).ToUpper();
                    }
                    else
                    {
                        _instance.InitialText.Text = "G";
                    }

                    _instance.AvatarCircle.Fill = new SolidColorBrush(GetColorFromName(user.Username));
                }

                _instance.Visibility = Visibility.Visible;

                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(100)));

                _instance.BeginAnimation(OpacityProperty, fadeIn);
            });
        }

        public static void HideOverlay()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_instance == null) return;

                var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(70)));
                fadeOut.Completed += (s, e) =>
                {
                    _instance.Visibility = Visibility.Collapsed;
                };

                _instance.BeginAnimation(OpacityProperty, fadeOut);
            });
        }

        private static Color GetColorFromName(string name)
        {
            int hash = name.GetHashCode();
            byte r = (byte)(hash & 0xFF);
            byte g = (byte)((hash >> 8) & 0xFF);
            byte b = (byte)((hash >> 16) & 0xFF);
            return Color.FromRgb(r, g, b);
        }
    }
}
