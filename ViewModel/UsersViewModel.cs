using Newtonsoft.Json.Linq;
using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.Services.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Parmigiano.ViewModel
{
    public class UsersViewModel : BaseView
    {
        private readonly IUserApiRepository _userApi = new UserApiRepository();

        private ObservableCollection<UserMinimalWithLMessageModel> _users;
        public ObservableCollection<UserMinimalWithLMessageModel> Users
        {
            get => _users;
            set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasUsers));
                }
            }
        }

        public bool HasUsers => Users != null && Users.Count > 0;

        public UsersViewModel()
        {
            Users = new ObservableCollection<UserMinimalWithLMessageModel>();
            Users.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasUsers));

            ConnectionService.Instance.OnWsEvent += HandleWebSocketEvent;
        }

        private void HandleWebSocketEvent(string evt, JObject data)
        {
            if (evt == Events.EVENT_USER_AVATAR_UPDATED)
            {
                ulong uid = 0;
                ulong.TryParse(data["user_uid"]?.ToString(), out uid);

                var avatarUrl = data["url"]?.ToString();

                var user = Users.FirstOrDefault(u => u.UserUid == uid);
                if (user != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        user.Avatar = avatarUrl;
                    });
                }
            }
            else if (evt == Events.EVENT_USER_NEW_REGISTER)
            {
                try
                {
                    var userObj = data.ToObject<UserMinimalWithLMessageModel>();
                    if (userObj != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (!Users.Any(u => u.UserUid == userObj.UserUid))
                            {
                                Users.Insert(0, userObj);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Ошибка обработки user_new_register: {ex.Message}");
                }
            }
        }

        public async Task LoadUsersAsync()
        {
            try
            {
                List<UserMinimalWithLMessageModel> users = await this._userApi.GetUsersMinimalWithLMessage();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Users.Clear();
                    foreach (var user in users)
                    {
                        Users.Add(user);
                    }
                });
            }
            catch (JsonException ex)
            {
                Logger.Error($"Ошибка парсинга JSON: {ex.Message}");
            }
        }
    }
}
