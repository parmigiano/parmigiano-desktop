using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.UI.Components;
using System;
using System.Reflection;
using System.Windows;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для Loader.xaml
    /// </summary>
    public partial class Loader : Window
    {
        private readonly IUserApiRepository _userApi = new UserApiRepository();
        private readonly IUserConfigRepository _userConfig = new UserConfigRepository();

        public Loader()
        {
            InitializeComponent();
            this.Loaded += Loader_Loaded;
        }

        private async void Loader_Loaded(object sender, RoutedEventArgs e)
        {
            // check update new version app
            this.CheckUpdate();

            string? userData = this._userConfig.GetString("access_token");

            try
            {
                if (!string.IsNullOrWhiteSpace(userData))
                {
                    try
                    {
                        UserInfoModel user = await this._userApi.GetUserMe();

                        if (user == null)
                        {
                            return;
                        }

                        if (!user.EmailConfirmed)
                        {
                            EmailConfirmedWindow emailConfirmedWindow = new();
                            emailConfirmedWindow.Show();
                            return;
                        }

                        MainWindow mainWindow = new();
                        mainWindow.Show();
                    }
                    catch (Exception ex)
                    {
                        Notification.Show("Ошибка", $"Ошибка при загрузке профиля: {ex.Message}", NotificationType.Error);

                        AuthWindow authWindow = new();
                        authWindow.Show();
                    }
                }
                else
                {
                    AuthWindow authWindow = new();
                    authWindow.Show();
                }
            }
            finally
            {
                this.Close();
            }
        }

        private async void CheckUpdate()
        {
            string currentVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            var (isUpdateAvailable, downloadUrl) = await UpdateAppService.CheckForUpdateAsync(currentVersion);

            if (isUpdateAvailable && !string.IsNullOrEmpty(downloadUrl))
            {
                var updateWindow = new UpdateAvailableWindow(downloadUrl);
                updateWindow.ShowDialog();
            }
        }
    }
}
