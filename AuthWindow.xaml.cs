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
            this._loginButtonText = LoginButton.Content.ToString() ?? "Войти";
        }

        #region OnSourceInitialized

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;
            const int BORDER_WIDTH = 8;

            if (msg == WM_NCHITTEST)
            {
                // Правильное извлечение координат с учётом 64-бит
                int x = (short)(lParam.ToInt64() & 0xFFFF);
                int y = (short)((lParam.ToInt64() >> 16) & 0xFFFF);

                Point pos = PointFromScreen(new Point(x, y));

                double width = ActualWidth;
                double height = ActualHeight;

                IntPtr result = IntPtr.Zero;

                // Верхняя граница
                if (pos.Y <= BORDER_WIDTH)
                {
                    if (pos.X <= BORDER_WIDTH) result = (IntPtr)HTTOPLEFT;
                    else if (pos.X >= width - BORDER_WIDTH) result = (IntPtr)HTTOPRIGHT;
                    else result = (IntPtr)HTTOP;
                }
                // Нижняя граница
                else if (pos.Y >= height - BORDER_WIDTH)
                {
                    if (pos.X <= BORDER_WIDTH) result = (IntPtr)HTBOTTOMLEFT;
                    else if (pos.X >= width - BORDER_WIDTH) result = (IntPtr)HTBOTTOMRIGHT;
                    else result = (IntPtr)HTBOTTOM;
                }
                // Левая/правая границы
                else if (pos.X <= BORDER_WIDTH) result = (IntPtr)HTLEFT;
                else if (pos.X >= width - BORDER_WIDTH) result = (IntPtr)HTRIGHT;

                if (result != IntPtr.Zero)
                {
                    handled = true; // сообщаем WPF, что событие обработано
                    return result;
                }
            }

            return IntPtr.Zero;
        }


        #endregion

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

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
