using Parmigiano.Services;
using Parmigiano.ViewModel;
using System;
using System.Diagnostics;
using System.Linq;
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

        protected override void OnStartup(StartupEventArgs e)
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
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ConnectionService.Instance.DisconnectAll();

            base.OnExit(e);
        }
    }
}
