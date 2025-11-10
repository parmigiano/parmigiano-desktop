using Parmigiano.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для UpdateAvailableWindow.xaml
    /// </summary>
    public partial class UpdateAvailableWindow : Window
    {
        private string _dwnlUpdate;

        public UpdateAvailableWindow(string downloadUrl)
        {
            InitializeComponent();

            this._dwnlUpdate = downloadUrl;
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

        private async void UpdButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await UpdateAppService.DownloadAndUpdateAsync(this._dwnlUpdate);
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
