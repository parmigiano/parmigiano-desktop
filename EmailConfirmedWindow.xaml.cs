using Newtonsoft.Json.Linq;
using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using System;
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

            // get me info
            LoadUser();

            // connect to websocket
            ConnectionService.Instance.ConnectWSocket();
            ConnectionService.Instance.OnWsEvent += HandleWebSocketEvent;
        }

        private async void LoadUser()
        {
            UserInfoModel user = await this._userApi.GetUserMe();

            // set uid in session
            AppSession.CurrentUserUid = user.UserUid;
        }

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

                MessageBox.Show("Письмо с подтверждением отправлено!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HandleWebSocketEvent(string evt, JObject data)
        {
            if (evt == Events.EVENT_AUTH_EMAIL_CONFIRMED)
            {
                ulong userUid = 0;
                ulong.TryParse(data["user_uid"]?.ToString(), out userUid);

                Logger.Info($"Email for {userUid} is confirmed");

                if (userUid == AppSession.CurrentUserUid)
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
    }
}
