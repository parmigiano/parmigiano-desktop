﻿using Microsoft.Win32;
using Parmigiano.Interface;
using Parmigiano.Repository;
using Parmigiano.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace Parmigiano.UI.Components
{
    /// <summary>
    /// Логика взаимодействия для UserMeCard.xaml
    /// </summary>
    public partial class UserMeCard : UserControl
    {
        private readonly IUserApiRepository _userApi = new UserApiRepository();

        public UserMeCard()
        {
            InitializeComponent();

            _ = this.LoadUserAsync();
        }

        private async Task LoadUserAsync()
        {
            try
            {
                var user = await this._userApi.GetUserMe();

                if (user == null)
                {
                    UsernameText.Text = "Гость";
                    return;
                }

                UsernameText.Text = user.Username;

                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    AvatarImage.ImageSource = new ImageSourceConverter().ConvertFromString(user.Avatar) as ImageSource;
                    InitialText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    AvatarImage.ImageSource = null;
                    InitialText.Visibility = Visibility.Visible;

                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        InitialText.Text = user.Email.Substring(0, 1).ToUpper();
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

                string? url = await this._userApi.UploadAvatar(filePath);

                if (url != null)
                {
                    var bitmap = new BitmapImage();

                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(url);
                    bitmap.EndInit();

                    AvatarImage.ImageSource = bitmap;
                    InitialText.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка: {ex.Message}");
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
    }
}
