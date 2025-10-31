using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Parmigiano
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IUserApiRepository _userApi = new UserApiRepository();

        private string _windowTitle = "Chat (Гость)";
        public string WindowTitle
        {
            get => this._windowTitle;
            set
            {
                this._windowTitle = value;
                OnPropertyChanged();
            }
        }

        private UserInfoModel? _selectedUser;
        public UserInfoModel? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            UsersListControl.UserSelected += OnUserSelected;

            _ = LoadUserAsync();
        }

        private async Task LoadUserAsync()
        {
            var user = await this._userApi.GetUserMe();
            if (user != null)
            {
                WindowTitle = $"Chat ({user.Username.ToLower()})";
            }
            else
            {
                WindowTitle = "Chat (Гость)";
            }
        }

        private void OnUserSelected(UserInfoModel user)
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
