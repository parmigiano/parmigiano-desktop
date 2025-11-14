using Microsoft.Win32;
using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Parmigiano.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для UserMySettingsModal.xaml
    /// </summary>
    public partial class UserMyProfileModal : UserControl
    {
        private static readonly IUserApiRepository _userApi = new UserApiRepository();
        private readonly IAuthApiRepository _authApi = new AuthApiRepository();
        private static ImageUtilities _imageUtilities = new ImageUtilities();

        private static UserMyProfileModal _instance;

        private readonly Dictionary<string, Timer> _debounceTimers = new();
        private const int DebounceDelay = 500;

        private readonly Dictionary<string, string> _originalValues = new();

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                _originalValues[tb.Name] = tb.Text;
            }
        }

        public UserMyProfileModal()
        {
            InitializeComponent();
            _instance = this;

            EmailToggle.Checked += this.Toggle_Checked;
            EmailToggle.Unchecked += this.Toggle_Unchecked;

            UsernameToggle.Checked += this.Toggle_Checked;
            UsernameToggle.Unchecked += this.Toggle_Unchecked;

            PhoneToggle.Checked += this.Toggle_Checked;
            PhoneToggle.Unchecked += this.Toggle_Unchecked;
        }

        private async void Toggle_Checked(object sender, RoutedEventArgs e)
        {
            await this.UpdateSingleToggle(sender as CheckBox, true);
        }

        private async void Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            await this.UpdateSingleToggle(sender as CheckBox, false);
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

        public static async void ShowProfile()
        {
            UserInfoModel user = await _userApi.GetUserMe();

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

        private async void AvatarGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Выберите фотографию",
                    Filter = "Изображения|*.png;*.jpg;*.jpeg;*.gif;*.bmp",
                    Multiselect = false
                };

                if (dialog.ShowDialog() != true)
                {
                    return;
                }

                string filePath = dialog.FileName;

                string? url = await _userApi.UploadAvatar(filePath);

                if (url != null)
                {
                    var bitmap = new BitmapImage();

                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(url);
                    bitmap.EndInit();

                    // reload in UserMeCard
                    AppSession.CurrentUser.Avatar = url;
                    UserMeCard.Reload();

                    AvatarCircle.Fill = new ImageBrush(bitmap)
                    {
                        Stretch = Stretch.UniformToFill
                    };

                    InitialText.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка: {ex.Message}");
            }
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox tb) return;

            string newText = tb.Text.Trim();

            if (string.IsNullOrEmpty(newText)) return;

            if (this._debounceTimers.TryGetValue(tb.Name, out Timer existingTimer))
            {
                existingTimer.Stop();
                existingTimer.Dispose();
            }

            var timer = new Timer(DebounceDelay)
            {
                AutoReset = false
            };

            string cachedName = tb.Name;
            string cachedValue = newText;

            timer.Elapsed += (_, _) =>
            {
                try
                {
                    var model = new UserProfileUpdModel
                    {
                        Overview = null,
                        Name = null,
                        Username = null,
                        UsernameVisible = null,
                        Email = null,
                        EmailVisible = true,
                        Phone = null,
                        PhoneVisible = null,
                        Password = null,
                    };

                    switch (cachedName)
                    {
                        case "NameTextBox":
                            model.Name = newText;
                            break;

                        case "UsernameTextBox":
                            model.Username = newText;
                            break;

                        case "PhoneTextBox":
                            model.Phone = newText;
                            break;

                        case "OverviewTextBox":
                            model.Overview = newText;
                            break;

                        case "EmailTextBox":
                            model.Email = newText;
                            break;

                        case "PasswordTextBox":
                            model.Password = newText;
                            break;

                        default:
                            return;
                    }

                    this._originalValues[cachedName] = cachedValue;

                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await _userApi.UpdateUserProfile(model);
                        UserMeCard.Reload();
                    });
                }
                catch (Exception ex)
                {
                    Logger.Error($"Ошибка при обновлении профиля: {ex.Message}");
                }
            };

            this._debounceTimers[tb.Name] = timer;
            timer.Start();
        }

        private async void DeleteAccount_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await this._authApi.AuthDelete();

            HideOverlay();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var authWindow = new AuthWindow();
                authWindow.Show();

                Application.Current.MainWindow.Close();
                Application.Current.MainWindow = authWindow;
            });
        }

        private async Task UpdateSingleToggle(CheckBox checkBox, bool isChecked)
        {
            if (checkBox == null) return;

            var model = new UserProfileUpdModel
            {
                EmailVisible = null,
                UsernameVisible = null,
                PhoneVisible = null,
            };


            switch (checkBox)
            {
                case var cb when cb == EmailToggle:
                    model.EmailVisible = isChecked;
                    break;
                case var cb when cb == UsernameToggle:
                    model.UsernameVisible = isChecked;
                    break;
                case var cb when cb == PhoneToggle:
                    model.PhoneVisible = isChecked;
                    break;
            }

            await _userApi.UpdateUserProfile(model);
        }
    }
}
