using Newtonsoft.Json.Linq;
using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.UI.Components;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для EmailConfirmedWindow.xaml
    /// </summary>
    public partial class EmailConfirmedWindow : Window
    {
        private readonly IAuthApiRepository _authApi = new AuthApiRepository();
        private readonly IUserApiRepository _userApi = new UserApiRepository();

        private bool _isConfirmStage = false;

        public EmailConfirmedWindow()
        {
            InitializeComponent();

            // connect to websocket
            ConnectionService.Instance.ConnectWSocket();
            ConnectionService.Instance.OnWsEvent += HandleWebSocketEvent;

            // new task
            _ = Task.Run(async () =>
            {
                await this._userApi.GetUserMe();
            });
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

        private async void EmailConfirmReq_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!this._isConfirmStage)
                {
                    EmailConfirm.Visibility = Visibility.Visible;
                    ReqConfirm.Text = "Отправить";
                    
                    this._isConfirmStage = true;
                    return;
                }

                ReqEmailConfirmModel model = new()
                {
                    Email = EmailBox.Text,
                };

                _ = await this._authApi.AuthEmailConfirmReq(model);

                EmailConfirm.Visibility = Visibility.Collapsed;
                ReqConfirm.Visibility = Visibility.Collapsed;

                Notification.Show("Успешно", $"Письмо с подтверждением отправлено!", NotificationType.Success);
            }
            catch (Exception ex)
            {
                Notification.Show("Ошибка", ex.Message, NotificationType.Error);
            }
        }

        private void HandleWebSocketEvent(string evt, JObject data)
        {
            if (evt == Events.EVENT_AUTH_EMAIL_CONFIRMED)
            {
                ulong userUid = 0;
                ulong.TryParse(data["user_uid"]?.ToString(), out userUid);

                Logger.Info($"Email for {userUid} is confirmed");

                if (userUid == AppSession.CurrentUser.UserUid)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (Window window in App.Current.Windows)
                        {
                            if (window is EmailConfirmedWindow emailWindow)
                            {
                                new MainWindow().Show();
                                emailWindow.Close();
                                break;
                            }
                        }
                    });
                }
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
