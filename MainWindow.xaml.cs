using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private WindowState _lastWindowState;

        private readonly IUserApiRepository _userApi = new UserApiRepository();
        private readonly IUserConfigRepository _userConfig = new UserConfigRepository();

        private string _windowTitle = "Гость";
        public string WindowTitle
        {
            get => this._windowTitle;
            set
            {
                this._windowTitle = value;
                OnPropertyChanged();
            }
        }

        private ChatMinimalWithLMessageModel? _selectedUser;
        public ChatMinimalWithLMessageModel? SelectedUser
        {
            get => _selectedUser;
            set
            {
                this._selectedUser = value;
                ChatControl.ViewModel.SelectedUser = value;

                ChatControl.Visibility = value != null ? Visibility.Visible : Visibility.Collapsed;
                PlaceholderText.Visibility = value == null ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            this.StateChanged += MainWindow_StateChanged;
            this._lastWindowState = this.WindowState;
            
            this.Activated += MainWindow_Activated;
            this.Deactivated += MainWindow_Deactivated;

            UsersListControl.UserSelected += OnUserSelected;

            _ = LoadUserAsync();

            ConnectionService.Instance.EnsureConnectedWSocket();
            ConnectionService.Instance.ConnectTcp();

            // notification
            NotificationService.StartNotification();

            // new task
            _ = Task.Run(() =>
            {
                if (!File.Exists(this._userConfig.GetString("rsa_private_key")) || !File.Exists(this._userConfig.GetString("rsa_public_key")))
                {
                    var keyService = new KeyService();
                }
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

        private async Task LoadUserAsync()
        {
            try
            {
                SkeletonOverlay.Visibility = Visibility.Visible;

                var user = AppSession.CurrentUser;
                if (user != null)
                {
                    WindowTitle = $"{user.Username.ToLower()}";
                }
                else
                {
                    WindowTitle = "Гость";
                }

                await UsersListControl.ViewModel.LoadChatsAsync();

                SkeletonOverlay.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка загрузки данных: {ex.Message}");
                SkeletonOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private void OnUserSelected(ChatMinimalWithLMessageModel user)
        {
            this.SelectedUser = user;
        }

        private async void MainWindow_StateChanged(object sender, EventArgs e)
        {
            bool currentOnline = this.WindowState != WindowState.Minimized;
            bool lastOnline = this._lastWindowState != WindowState.Minimized;

            if (currentOnline != lastOnline)
            {
                await TcpSendPacketsService.SendOnlinePacketAsync(currentOnline);
            }
            this._lastWindowState = this.WindowState;
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void MainWindow_Activated(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                await TcpSendPacketsService.SendOnlinePacketAsync(true);
            }
        }

        private async void MainWindow_Deactivated(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                await TcpSendPacketsService.SendOnlinePacketAsync(false);
            }
        }
    }
}
