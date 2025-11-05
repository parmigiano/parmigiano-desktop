using Parmigiano.Interface;
using Parmigiano.Models;
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
using System.Windows.Shapes;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private readonly IUserConfigRepository _userConfig = new UserConfigRepository();
        private readonly IAuthApiRepository _authApi = new AuthApiRepository();

        private string _registerButtonText = "Создать аккаунт";
        private string _loginButtonText = "Войти";

        public AuthWindow()
        {
            InitializeComponent();

            this._registerButtonText = RegisterButton.Content.ToString() ?? "Создать аккаунт";
            this._registerButtonText = LoginButton.Content.ToString() ?? "Войти";
        }

        private void LoginLink_Click(object sender, MouseButtonEventArgs e)
        {
            RegisterPanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
        }

        private void RegisterLink_Click(object sender, MouseButtonEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Visible;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegisterButton.Content = null;

                RegisterButton.IsEnabled = false;
                LoaderRegister.Start();

                var model = new AuthCreateModel
                {
                    Name = NameBox.Text,
                    Username = UsernameBox.Text,
                    Email = EmailBox.Text,
                    Password = PasswordBbox.Password,
                };

                string? result = await this._authApi.AuthCreate(model);

                if (!string.IsNullOrEmpty(result))
                {
                    this._userConfig.Set("access_token", result);

                    var emailConfirmWindow = new EmailConfirmedWindow();
                    emailConfirmWindow.Show();

                    this.Close();
                }
            }
            finally
            {
                RegisterButton.IsEnabled = true;
                LoaderRegister.Stop();
    
                RegisterButton.Content = this._registerButtonText;
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginButton.Content = null;

                LoginButton.IsEnabled = false;
                LoaderLogin.Start();

                var model = new AuthLoginModel
                {
                    Email = EmailLoginBox.Text,
                    Password = PasswordLoginBox.Password,
                };

                string? result = await this._authApi.AuthLogin(model);

                if (!string.IsNullOrEmpty(result))
                {
                    this._userConfig.Set("access_token", result);

                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    this.Close();
                }
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoaderLogin.Stop();

                LoginButton.Content = this._loginButtonText;
            }
        }
    }
}
