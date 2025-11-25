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
        private System.Threading.CancellationTokenSource _searchCts;

        private readonly IChatApiRepository _chatApi = new ChatApiRepository();
        private readonly IUserApiRepository _userApi = new UserApiRepository();

        private string _searchText;
        public string SearchText
        {
            get => this._searchText;
            set
            {
                if (this._searchText != value)
                {
                    this._searchText = value;
                    OnPropertyChanged();

                    this.StartSearchDebounce();
                }
            }
        }

        private bool _hasSearchResults = true;
        public bool HasSearchResults
        {
            get => this._hasSearchResults;
            set
            {
                if (this._hasSearchResults != value)
                {
                    this._hasSearchResults = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ChatMinimalWithLMessageModel> _users;
        public ObservableCollection<ChatMinimalWithLMessageModel> Users
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
            Users = new ObservableCollection<ChatMinimalWithLMessageModel>();
            Users.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasUsers));

            ConnectionService.Instance.OnWsEvent += HandleWebSocketEvent;
            ConnectionService.Instance.OnTcpEvent += HandleTcpEvent;
        }

        private void HandleWebSocketEvent(string evt, JObject data)
        {
            if (evt == Events.EVENT_USER_NEW_MESSAGE)
            {
                try
                {
                    ulong chatId = data["chat_id"]?.ToObject<ulong>() ?? 0;
                    string content = data["content"]?.ToString() ?? "";
                    DateTime now = DateTime.Now;

                    var user = Users.FirstOrDefault(u => u.Id == chatId || u.UserUid == (data["sender_uid"]?.ToObject<ulong>() ?? 0));
                    if (user != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            user.LastMessage = content;
                            user.LastMessageDate = now;

                            if (user.UserUid != AppSession.CurrentUser.UserUid)
                            {
                                user.UnreadMessageCount++;
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Ошибка обработки new_message: {ex.Message}");
                }
            }

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
                    var userObj = data.ToObject<ChatMinimalWithLMessageModel>();
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
            else if (evt == Events.EVENT_USER_ONLINE)
            {
                try
                {
                    ulong uid = data["user_uid"]?.ToObject<ulong>() ?? 0;
                    bool online = data["online"]?.ToObject<bool>() ?? false;

                    var user = Users.FirstOrDefault(u => u.UserUid == uid);
                    if (user == null) return;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        user.Online = online;
                        user.LastOnlineDate = DateTime.Now;
                    });
                }
                catch (Exception ex)
                {
                    Logger.Error($"Ошибка обработки user_online: {ex.Message}");
                }
            }
        }

        private void HandleTcpEvent(ResponseStruct.Response response)
        {
            try
            {
                switch (response)
                {
                    // client active packet
                    case { ClientActive: not null }:
                        if (response?.ClientActive == null) return;

                        ulong uid = response.ClientActive.Uid;
                        bool online = response.ClientActive.Online;

                        var user = Users.FirstOrDefault(u => u.UserUid == uid);
                        if (user == null) return;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            user.Online = online;
                            user.LastOnlineDate = DateTime.Now;
                        });

                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"HandleTcpEvent error: {ex.Message}");
            }
        }

        private void StartSearchDebounce()
        {
            this._searchCts?.Cancel();
            this._searchCts = new System.Threading.CancellationTokenSource();

            var token = this._searchCts.Token;

            Task.Delay(500).ContinueWith(async t =>
            {
                if (token.IsCancellationRequested) return;

                await this.SearchUsersAsync(this._searchText);
            }, token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        private async Task SearchUsersAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                await this.LoadChatsAsync();
                return;
            }

            try
            {
                var results = await this._userApi.GetUsersFindByUsername(query);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Users.Clear();

                    foreach (var user in results)
                    {
                        Users.Add(user);
                    }

                    HasSearchResults = Users.Count > 0;
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка поиска пользователей: {ex.Message}");
            }
        }

        public async Task LoadChatsAsync()
        {
            try
            {
                List<ChatMinimalWithLMessageModel> chats = await this._chatApi.GetChats();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Users.Clear();
                    foreach (var chat in chats)
                    {
                        Users.Add(chat);
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
