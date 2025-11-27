using Newtonsoft.Json.Linq;
using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.Services.Wpf;
using Parmigiano.UI.Components;
using ResponseStruct;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Parmigiano.ViewModel
{
    public class ChatViewModel : BaseView
    {
        private readonly IChatApiRepository _chatApi = new ChatApiRepository();

        public event Action ChatSettingUpdated;
        public event Action<ulong> MessageReadInChat;

        private System.Timers.Timer _typingTimer;

        private bool _isLoadingOlder = false;
        private int _currentOffset = 0;

        public ICommand SendMessageCommand { get; }
        public ICommand EditMessageCommand { get; }
        public ICommand DeleteMessageCommand { get; }
        public ICommand CopyMessageCommand { get; }

        private string _messageText;
        public string MessageText
        {
            get => _messageText;
            set
            {
                this._messageText = value;
                OnPropertyChanged(nameof(MessageText));

                _ = this.SendTypingStatus();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                this._isLoading = value;
                OnPropertyChanged();
            }
        }

        private OnesMessageModel _editingMessage;
        public OnesMessageModel EditingMessage
        {
            get => _editingMessage;
            set
            {
                this._editingMessage = value;
                OnPropertyChanged(nameof(EditingMessage));

                if (this._editingMessage != null)
                {
                    this.MessageText = this._editingMessage.IsEdited ? this._editingMessage.EditContent : this._editingMessage.Content ?? string.Empty;
                }
                else
                {
                    this.MessageText = string.Empty;
                }
            }
        }

        public ObservableCollection<OnesMessageModel> Messages { get; set; } = new();
        public ChatSettingModel ChatSetting { get; set; } = new();

        private ChatMinimalWithLMessageModel _selectedUser;
        public ChatMinimalWithLMessageModel SelectedUser
        {
            get => this._selectedUser;
            set
            {
                if (this._selectedUser != value)
                {
                    this._selectedUser = value;
                    OnPropertyChanged();

                    this._currentOffset = 0;
                    this._isLoadingOlder = false;
                    this.Messages.Clear();

                    this.LoadMessagesAsync();
                }
            }
        }

        public ChatViewModel()
        {
            this.SendMessageCommand = new RelayCommand(async _ => await this.SendMessage());
            this.EditMessageCommand = new RelayCommand(msg => this.EditMessage(msg));
            this.DeleteMessageCommand = new RelayCommand(async msg => await this.DeleteMessage(msg));
            this.CopyMessageCommand = new RelayCommand(msg => this.CopyMessage(msg));

            ConnectionService.Instance.OnWsEvent += HandleWSocketEvent;
            ConnectionService.Instance.OnTcpEvent += HandleTcpEvent;
        }

        private void InitializeTypingTimer()
        {
            if (this._typingTimer != null)
            {
                return;
            }

            this._typingTimer = new System.Timers.Timer(3000);
            this._typingTimer.AutoReset = false;
            this._typingTimer.Elapsed += async (s, e) =>
            {
                if (this.SelectedUser != null)
                {
                    await TcpSendPacketsService.SendTypingAsync(this.SelectedUser.Id, false);
                }
            };
        }

        private async void LoadMessagesAsync()
        {
            if (this.SelectedUser == null)
            {
                return;
            }

            this.IsLoading = true;

            try
            {
                this.Messages.Clear();

                List<OnesMessageModel> messages = await this._chatApi.GetPrivateChatHistory(this.SelectedUser.UserUid, this._currentOffset);

                ChatSettingModel chatSetting = await this._chatApi.GetChatSetting(this.SelectedUser.Id);

                this.ChatSetting.ChatId = chatSetting.ChatId;
                this.ChatSetting.Blocked = chatSetting.Blocked;
                this.ChatSetting.CustomBackground = chatSetting.CustomBackground;

                this.ChatSettingUpdated?.Invoke();

                foreach (var message in messages)
                {
                    if (!Messages.Any(m => m.Id == message.Id))
                    {
                        message.IsMine = message.SenderUid == AppSession.CurrentUser.UserUid;
                        Messages.Add(message);
                    }
                }

                this._currentOffset = this.Messages.Count;
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        #region LOAD MORE MESSAGE +=30

        public async Task LoadOlderMessagesAsync()
        {
            //if (this._isLoadingOlder) return;

            //this._isLoadingOlder = true;

            //try
            //{
            //    var olderMessages = await this._chatApi.GetPrivateChatHistory(this.SelectedUser.UserUid, this._currentOffset);
            //    if (olderMessages != null && olderMessages.Any())
            //    {
            //        foreach (var msg in olderMessages)
            //        {
            //            if (!Messages.Any(m => m.Id == msg.Id))
            //            {
            //                msg.IsMine = msg.SenderUid == AppSession.CurrentUser.UserUid;
            //                Messages.Insert(0, msg);
            //            }
            //        }

            //        this._currentOffset += olderMessages.Count;
            //    }
            //}
            //finally
            //{
            //    this._isLoadingOlder = false;
            //}
            Console.WriteLine("Load older messages");
        }

        #endregion

        private void HandleWSocketEvent(string evt, JObject data)
        {
            if (evt == Events.EVENT_CHAT_BACKGROUND_UPDATED)
            {
                if (this.ChatSetting == null) return;

                ulong chatId = 0;
                ulong.TryParse(data["chat_id"]?.ToString(), out chatId);

                var customBackground = data["url"]?.ToString();

                if (this.ChatSetting.ChatId == chatId)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.ChatSetting.CustomBackground = customBackground;
                        this.ChatSettingUpdated?.Invoke();
                    });
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
                        this.packetReceivedOnlineMessage(response);
                        break;

                    // client received message packet
                    case { ClientSendMessage: not null }:
                        this.packetReceivedMessageTcp(response);
                        break;

                    // client received delete message packet
                    case { ClientDeleteMessage: not null }:
                        this.packetDeleteMessageTcp(response);
                        break;

                    // client received read message packet
                    case { ClientReadMessage: not null }:
                        this.packetMarkReadMessageTcp(response);
                        break;

                    // client received edit message packet
                    case { ClientEditMessage: not null }:
                        this.packetEditMessageTcp(response);
                        break;

                    // client received typing user packet
                    case { ClientTyping: not null }:
                        this.packetTypingTcp(response);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("HandleTcpEvent error: " + ex.Message);
            }
        }

        // COMMANDS
        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(this.MessageText)) return;

            string text = this.MessageText;

            string prepared = MessageService.PreprocessMessage(text);
            if (string.IsNullOrWhiteSpace(prepared))
            {
                this.MessageText = string.Empty;
                return;
            }

            if (this.EditingMessage != null)
            {
                this.EditingMessage.EditContent = prepared;
                this.EditingMessage.IsEdited = true;

                try
                {
                    await TcpSendPacketsService.SendEditMessageAsync(this.EditingMessage.ChatId, this.EditingMessage.Id, prepared, this.EditingMessage.ContentType);
                }
                catch (Exception ex)
                {
                    Logger.Tcp("SendEdit failed: " + ex.Message);
                }

                this.EditingMessage = null;
                this.MessageText = string.Empty;

                return;
            }

            if (prepared.Length > MessageService.MaxMessageLength)
            {
                prepared = prepared.Substring(0, MessageService.MaxMessageLength);
            }

            var parts = new List<string>();
            for (int i = 0; i < prepared.Length; i += MessageService.MaxMessagePartLength)
            {
                int len = Math.Min(MessageService.MaxMessagePartLength, prepared.Length - i);
                parts.Add(prepared.Substring(i, len));
            }

            Random rnd = new Random();
            var chatId = this.SelectedUser?.Id ?? 0UL;

            foreach (var part in parts)
            {
                ulong tmpMessageId = ulong.Parse(new string(Enumerable.Range(0, 18).Select(_ => (char)('0' + rnd.Next(10))).ToArray()));

                var optimistic = new OnesMessageModel
                {
                    Id = tmpMessageId,
                    SenderUid = AppSession.CurrentUser.UserUid,
                    ChatId = chatId,
                    Content = part,
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = part,
                    DeliveredAt = DateTime.Now,
                    ReadAt = null,
                    IsMine = true,
                };

                this.Messages.Add(optimistic);

                try
                {
                    await MessageService.SendMessageAsync(chatId, tmpMessageId, part);
                }
                catch (Exception ex)
                {
                    Logger.Tcp("SendMessage failed: " + ex.Message);
                }
            }

            this.MessageText = string.Empty;
        }

        private void EditMessage(object msg)
        {
            if (msg is not OnesMessageModel message)
            {
                return;
            }

            if (!message.IsMine)
            {
                return;
            }

            if (this.EditingMessage == message)
            {
                this.EditingMessage = null;
                this.MessageText = string.Empty;
                return;
            }

            this.EditingMessage = message;

            this.MessageText = message.IsEdited ? message.EditContent ?? "" : message.Content ?? "";
        }

        private async Task DeleteMessage(object msg)
        {
            if (msg is OnesMessageModel message && this.Messages.Contains(message) && message.IsMine)
            {
                this.Messages.Remove(message);

                try
                {
                    await TcpSendPacketsService.SendDeleteMessageAsync(message.ChatId, message.Id);
                }
                catch (Exception ex)
                {
                    Logger.Tcp("SendDeleteMessage failed: " + ex.Message);
                }
            }
        }

        private void CopyMessage(object msg)
        {
            try
            {
                if (msg is OnesMessageModel message)
                {
                    Clipboard.SetText(message.IsEdited ? message.EditContent : message.Content ?? string.Empty);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка копирования текста: " + ex.Message);
            }   
        }

        public async Task SendTypingStatus()
        {
            try
            {
                if (this.SelectedUser == null) return;

                this.InitializeTypingTimer();

                if (!this.SelectedUser.IsTyping)
                {
                    await TcpSendPacketsService.SendTypingAsync(this.SelectedUser.Id, true);
                }

                this._typingTimer.Stop();
                this._typingTimer.Start();
            }
            catch (Exception ex)
            {
                Logger.Error($"SendTypingStatus: error -> {ex.Message}");
            }
        }

        public async Task MarkMessageAsRead(OnesMessageModel message)
        {
            if (this.SelectedUser == null) return;

            try
            {
                await TcpSendPacketsService.SendReadMessageAsync(message.ChatId, message.Id);
                MessageReadInChat?.Invoke(message.ChatId);
            }
            catch (Exception ex)
            {
                Logger.Error($"MarkMessageAsRead: error -> {ex.Message}");
            }
        }

        #region PACKET RECEIVED MESSAGE

        private void packetReceivedMessageTcp(ResponseStruct.Response response)
        {
            try
            {
                if (response?.ClientSendMessage == null) return;

                var packet = response.ClientSendMessage;

                ulong tmpMessageId = packet.TempMessageId;
                ulong messageId = packet.MessageId;
                ulong chatId = packet.ChatId;
                ulong senderUid = packet.SenderUid;
                string content = packet.Content;
                string contentType = packet.ContentType;
                string deliveredAt = packet.DeliveredAt;

                DateTime deliveredTime;
                if (!DateTime.TryParse(deliveredAt, out deliveredTime))
                {
                    deliveredTime = DateTime.Now;
                }

                string title = "Новое сообщение";

                if (this.SelectedUser != null && this.SelectedUser.UserUid == senderUid)
                {
                    title = string.IsNullOrEmpty(this.SelectedUser.Name) ? this.SelectedUser.Username : this.SelectedUser.Name;
                }

                string avatar = null;

                if (this.SelectedUser != null && this.SelectedUser.UserUid == senderUid)
                {
                    avatar = string.IsNullOrEmpty(this.SelectedUser.Avatar) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Public/assets/logo-bg-x250.png") : this.SelectedUser.Avatar;
                }
                else
                {
                    avatar = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Public/assets/logo-bg-x250.png");
                }

                string notifyPayload = $"{title}|{content}|{chatId}|{avatar}";

                // send notification
                // is not me sender
                if (senderUid != AppSession.CurrentUser.UserUid)
                {
                    _ = NotificationService.NotifyAsync(notifyPayload);
                }

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var existingMsg = this.Messages.FirstOrDefault(m => m.Id == tmpMessageId);

                        if (existingMsg != null)
                        {
                            // update all field
                            existingMsg.Id = messageId;
                            existingMsg.SenderUid = senderUid;
                            existingMsg.ChatId = chatId;
                            existingMsg.Content = content;
                            existingMsg.ContentType = contentType;
                            existingMsg.IsEdited = false;
                            existingMsg.EditContent = content;
                            existingMsg.DeliveredAt = deliveredTime;
                            existingMsg.ReadAt = null;
                            existingMsg.IsMine = senderUid == AppSession.CurrentUser.UserUid;
                        }
                        else if (this.SelectedUser != null && (this.SelectedUser.Id == chatId || this.SelectedUser.UserUid == senderUid))
                        {
                            // added new message
                            var msg = new OnesMessageModel
                            {
                                Id = messageId,
                                SenderUid = senderUid,
                                ChatId = chatId,
                                Content = content,
                                ContentType = contentType,
                                IsEdited = false,
                                EditContent = content,
                                DeliveredAt = deliveredTime,
                                ReadAt = null,
                                IsMine = senderUid == AppSession.CurrentUser.UserUid,
                            };

                            this.Messages.Add(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("HandleTcpEvent UI update failed: " + ex.Message);
                    }
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("HandleTcpEvent error: " + ex.Message);
            }
        }

        #endregion

        #region PACKET RECEIVED DELETE MESSAGE

        private void packetDeleteMessageTcp(ResponseStruct.Response response)
        {
            try
            {
                if (response?.ClientDeleteMessage == null) return;

                var packet = response.ClientDeleteMessage;

                ulong messageId = packet.MessageId;
                ulong chatId = packet.ChatId;

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var msg = this.Messages.FirstOrDefault(m => m.Id == messageId);
                        if (msg != null)
                        {
                            this.Messages.Remove(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("DeleteMessageFromList error: " + ex.Message);
                    }
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("HandleTcpEvent error: " + ex.Message);
            }
        }

        #endregion

        #region PACKET RECEIVED MARK IS READ MESSAGE

        private void packetMarkReadMessageTcp(ResponseStruct.Response response)
        {
            try
            {
                if (response?.ClientReadMessage == null) return;

                var packet = response.ClientReadMessage;

                ulong messageId = packet.MessageId;
                ulong chatId = packet.ChatId;

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var msg = this.Messages.FirstOrDefault(m => m.Id == messageId);
                        if (msg != null)
                        {
                            msg.ReadAt = DateTime.Now;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("MarkReadMessageFromList error: " + ex.Message);
                    }
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("HandleTcpEvent error: " + ex.Message);
            }
        }

        #endregion

        #region PACKET EDIT MESSAGE

        private void packetEditMessageTcp(ResponseStruct.Response response)
        {
            try
            {
                if (response?.ClientEditMessage == null) return;

                var packet = response.ClientEditMessage;

                ulong messageId = packet.MessageId;
                ulong chatId = packet.ChatId;
                ulong senderUid = packet.SenderUid;
                string content = packet.Content;
                string contentType = packet.ContentType;
                string deliveredAt = packet.DeliveredAt;

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var msg = this.Messages.FirstOrDefault(m => m.Id == messageId);
                        if (msg != null)
                        {
                            msg.Content = content;
                            msg.EditContent = content;
                            msg.ContentType = contentType;

                            if (DateTime.TryParse(deliveredAt, out DateTime parsedDate))
                            {
                                msg.DeliveredAt = parsedDate;
                            }

                            msg.IsEdited = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("EditMessageFromList error: " + ex.Message);
                    }
                }));
            }
            catch (Exception ex)
            {
                Logger.Error("HandleTcpEvent error: " + ex.Message);
            }
        }

        #endregion

        #region PACKET RECEIVED ONLINE USER

        private void packetReceivedOnlineMessage(ResponseStruct.Response response)
        {
            try
            {
                if (response?.ClientActive == null) return;

                ulong uid = response.ClientActive.Uid;
                bool online = response.ClientActive.Online;

                if (this.SelectedUser == null) return;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.SelectedUser.Online = online;
                    this.SelectedUser.LastOnlineDate = DateTime.Now;
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"HandleTcpEvent error: {ex.Message}");
            }
        }

        #endregion

        #region PACKET RECEIVED TYPING

        private void packetTypingTcp(ResponseStruct.Response response)
        {
            try
            {
                if (response?.ClientTyping == null) return;

                ulong uid = response.ClientTyping.Uid;
                bool typing = response.ClientTyping.IsTyping;

                if (uid == AppSession.CurrentUser.UserUid) return;

                if (this.SelectedUser == null || this.SelectedUser.UserUid != uid)
                {
                    return;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.SelectedUser.IsTyping = typing;
                });
            }
            catch (Exception ex)
            {
                Logger.Error("TypingMessageFromList error: " + ex.Message);
            }
        }

        #endregion
    }
}
