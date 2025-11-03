using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IUserApiRepository _userApi = new UserApiRepository();

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var userConfigRepo = new UserConfigRepository();
            string userData = userConfigRepo.Get("access_token");

            if (!string.IsNullOrWhiteSpace(userData))
            {
                try
                {
                    UserInfoModel user = await this._userApi.GetUserMe();

                    if (user == null)
                    {
                        AuthWindow authWindow = new();
                        authWindow.Show();
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

        protected override void OnExit(ExitEventArgs e)
        {
            ConnectionService.Instance.DisconnectAll();

            base.OnExit(e);
        }
    }
}
