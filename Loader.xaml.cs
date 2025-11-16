using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.UI.Components;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для Loader.xaml
    /// </summary>
    public partial class Loader : Window
    {
        private readonly NetworkService _networkService = new NetworkService();
        private readonly IUserApiRepository _userApi = new UserApiRepository();
        private readonly IUserConfigRepository _userConfig = new UserConfigRepository();

        public Loader()
        {
            InitializeComponent();
            this.Loaded += Loader_Loaded;
        }

        private async void Loader_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this._networkService.IsAvailable)
            {
                Notification.Show("Ошибка сети", "Нет подключения к интернету. Проверьте соединение и попробуйте снова.", NotificationType.Error);

                await Task.Delay(5000);
                Application.Current.Shutdown();
                return;
            }

            string? userData = this._userConfig.GetString("access_token");

            try
            {
                // check update new version app
                var (isUpdateAvailable, downloadUrl) = await this.CheckUpdate();
                if (isUpdateAvailable && !string.IsNullOrEmpty(downloadUrl))
                {
                    this.Hide();

                    var updateWindow = new UpdateAvailableWindow(downloadUrl);
                    Application.Current.MainWindow = updateWindow;
                    updateWindow.ShowDialog();

                    this.Close();
                    return;
                }

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

        private async Task<(bool isUpdateAvailable, string downloadUrl)> CheckUpdate()
        {
            string currentVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return await UpdateAppService.CheckForUpdateAsync(currentVersion);
        }
    }
}
