using Parmigiano.Repository;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var userConfigRepo = new UserConfigRepository();
            string userData = userConfigRepo.Get("access_token");

            if (!string.IsNullOrWhiteSpace(userData))
            {
                MainWindow mainWindow = new();
                mainWindow.Show();
            }
            else
            {
                AuthWindow authWindow = new();
                authWindow.Show();
            }
        }
    }
}
