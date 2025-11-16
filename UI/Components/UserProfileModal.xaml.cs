using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Parmigiano.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для UserProfileModal.xaml
    /// </summary>
    public partial class UserProfileModal : UserControl
    {
        private static readonly IUserApiRepository _userApi = new UserApiRepository();
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

        public static async Task ShowProfile(ulong uid)
        {
            if (_instance == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_instance == null) return;
                _instance.UsernameText.Text = string.Empty;
                _instance.NameText.Text = string.Empty;
                _instance.OverviewText.Text = string.Empty;
                _instance.EmailText.Text = string.Empty;
                _instance.PhoneText.Text = string.Empty;

                _instance.AvatarImage.ImageSource = null;
                _instance.InitialText.Text = string.Empty;
                _instance.AvatarCircle.Fill = new SolidColorBrush(Colors.Transparent);

                _instance.Visibility = Visibility.Visible;
                var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(100)));
                _instance.BeginAnimation(OpacityProperty, fadeIn);
            });

            UserInfoModel user = await _userApi.GetUserProfile(uid);

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_instance == null || user == null) return;

                _instance.DataContext = user;

                _instance.UsernameText.Text = user.Username;
                _instance.NameText.Text = user.Name;
                _instance.OverviewText.Text = user.Overview; 
                _instance.EmailText.Text = user.Email; 
                _instance.PhoneText.Text = user.Phone; 

                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    _imageUtilities.LoadImageAsync(user.Avatar, _instance.AvatarImage);
                    _instance.AvatarCircle.Fill = _instance.AvatarImage;
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

        private void Avatar_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var imgSource = this.AvatarImage?.ImageSource;
                if (imgSource == null)
                {
                    return;
                }

                var viewer = new AvatarViewerWindow(imgSource);

                var owner = Window.GetWindow(this);
                if (owner != null)
                {
                    viewer.Owner = owner;
                    viewer.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }

                viewer.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Error("UserProfileModal.Avatar_Click: " + ex.Message);
            }
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
