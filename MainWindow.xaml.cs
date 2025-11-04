using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.UI.Components;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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

        private UserMinimalWithLMessageModel? _selectedUser;
        public UserMinimalWithLMessageModel? SelectedUser
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

            UsersListControl.UserSelected += OnUserSelected;

            _ = LoadUserAsync();

            ConnectionService.Instance.EnsureConnectedWSocket();
            ConnectionService.Instance.ConnectTcp();

            // new task
            _ = Task.Run(() =>
            {
                if (!File.Exists(this._userConfig.Get("rsa_private_key")) || !File.Exists(this._userConfig.Get("rsa_public_key")))
                {
                    var keyService = new KeyService();
                }
            });
        }

        private async Task LoadUserAsync()
        {
            try
            {
                SkeletonOverlay.Visibility = Visibility.Visible;

                var user = await this._userApi.GetUserMe();
                if (user != null)
                {
                    WindowTitle = $"{user.Username.ToLower()}";
                }
                else
                {
                    WindowTitle = "Гость";
                }

                await UsersListControl.ViewModel.LoadUsersAsync();

                SkeletonOverlay.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка загрузки данных: {ex.Message}");
                SkeletonOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private void OnUserSelected(UserMinimalWithLMessageModel user)
        {
            this.SelectedUser = user;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
