using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private readonly IUserApiRepository _userApi = new UserApiRepository();

        protected override async void OnStartup(StartupEventArgs e)
        {
            const string appName = "ParmigianoChatUniqueMutexName";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                var existing = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).FirstOrDefault(p => p.Id != Process.GetCurrentProcess().Id);

                if (existing != null)
                {
                    SetForegroundWindow(existing.MainWindowHandle);
                }

                Current.Shutdown();
                return;
            }

            // start app
            base.OnStartup(e);

            // check update new version app
            this.CheckUpdate();

            var userConfigRepo = new UserConfigRepository();
            string? userData = userConfigRepo.GetString("access_token");

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
                    MessageBox.Show("Ошибка при загрузке профиля: " + ex.Message);

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

        protected override void OnExit(ExitEventArgs e)
        {
            ConnectionService.Instance.DisconnectAll();

            base.OnExit(e);
        }
    }
}
