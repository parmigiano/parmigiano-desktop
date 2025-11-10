using Microsoft.Win32;
using Parmigiano.Core;
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
using System.Windows.Media.Imaging;

namespace Parmigiano.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для UserMeCard.xaml
    /// </summary>
    public partial class UserMeCard : UserControl
    {
        private readonly IUserApiRepository _userApi = new UserApiRepository();
        private readonly ImageUtilities _imageUtilities = new ImageUtilities();
        private readonly IUserConfigRepository _userConfig = new UserConfigRepository();

        private static UserMeCard? _instance;

        public UserMeCard()
        {
            InitializeComponent();

            _instance = this;

            this.LoadUserAsync();
        }

        private async void LoadUserAsync()
        {
            try
            {
                UserInfoModel? user = AppSession.CurrentUser;

                if (user == null)
                {
                    UserInfoModel? userPrepare = await this._userApi.GetUserMe();
                    if (userPrepare == null)
                    {
                        Notification.Show("Ошибка получения профиля", "Профиль не найден, перезапустите приложение", NotificationType.Error);
                        return;
                    }

                    AppSession.CurrentUser = userPrepare;
                    user = userPrepare;
                }

                UsernameText.Text = $"@{user.Username}";
                NameText.Text = user.Name;

                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    this._imageUtilities.LoadImageAsync(user.Avatar, AvatarImage);
                    InitialText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    AvatarImage.ImageSource = null;
                    InitialText.Visibility = Visibility.Visible;

                    if (!string.IsNullOrEmpty(user.Username))
                    {
                        InitialText.Text = user.Username.Substring(0, 1).ToUpper();
                    }
                    else
                    {
                        InitialText.Text = "G";
                    }

                    AvatarCircle.Fill = new SolidColorBrush(GetColorFromName(user.Username));
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[UserMeCard] {ex.Message}");
            }
        }

        private Color GetColorFromName(string name)
        {
            int hash = name.GetHashCode();
            byte r = (byte)(hash & 0xFF);
            byte g = (byte)((hash >> 8) & 0xFF);
            byte b = (byte)((hash >> 16) & 0xFF);
            return Color.FromRgb(r, g, b);
        }

        private void Logout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this._userConfig.DeleteKey("access_token");

            var authWindow = new AuthWindow();
            authWindow.Show();

            Window.GetWindow(this)?.Close();
        }

        private void UserProfile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UserMyProfileModal.ShowProfile(AppSession.CurrentUser);
        }

        public void ReloadUser()
        {
            LoadUserAsync();
        }

        public static void Reload()
        {
            _instance?.ReloadUser();
        }
    }
}
