using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.Services.Wpf;
using ResponseStruct;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Parmigiano.ViewModel
{
    public class ChatViewModel : BaseView
    {
        private readonly IChatApiRepository _chatApi = new ChatApiRepository();

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
                    this.MessageText = this._editingMessage.IsEdited
                        ? this._editingMessage.EditContent
                        : this._editingMessage.Content ?? string.Empty;
                }
                else
                {
                    this.MessageText = string.Empty;
                }
            }
        }

        public ObservableCollection<OnesMessageModel> Messages { get; set; } = new();

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

                    this.LoadMessagesAsync();
                }
            }
        }

        public ChatViewModel()
        {
            this.SendMessageCommand = new RelayCommand(async _ => await this.SendMessage());
            this.EditMessageCommand = new RelayCommand(msg => this.EditMessage(msg));
            this.DeleteMessageCommand = new RelayCommand(msg => this.DeleteMessage(msg));
            this.CopyMessageCommand = new RelayCommand(msg => this.CopyMessage(msg));

            ConnectionService.Instance.OnTcpEvent += HandleTcpEvent;
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

                List<OnesMessageModel> messages = await this._chatApi.GetHistory(this.SelectedUser.UserUid);

                foreach (var message in messages)
                {
                    message.IsMine = message.SenderUid == AppSession.CurrentUser.UserUid;
                }

                foreach (var message in messages)
                {
                    this.Messages.Add(message);
                }
            }
            finally
            {
                this.IsLoading = false; 
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
            }
            else
            {
                var optimistic = new OnesMessageModel
                {
                    SenderUid = AppSession.CurrentUser.UserUid,
                    ChatId = this.SelectedUser?.Id ?? 0UL,
                    Content = prepared,
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = prepared,
                    DeliveredAt = DateTime.Now,
                    ReadAt = null,
                    IsMine = true,
                };

                this.Messages.Add(optimistic);

                this.MessageText = string.Empty;

                try
                {
                    var chatId = this.SelectedUser?.Id ?? 0UL;
                    await MessageService.SendMessageAsync(chatId, prepared);
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

        #region PACKET RECEIVED MESSAGE

        private void packetReceivedMessageTcp(ResponseStruct.Response response)
        {
            try
            {
                if (response?.ClientSendMessage == null) return;

                var packet = response.ClientSendMessage;

                ulong messageId = packet.MessageId;
                ulong chatId = packet.ChatId;
                ulong senderUid = packet.SenderUid;
                string content = packet.Content;
                string contentType = packet.ContentType;
                string deliveredAt = packet.DeliveredAt;

                string title = "Новое сообщение";

                if (this.SelectedUser != null && this.SelectedUser.UserUid == senderUid)
                {
                    title = string.IsNullOrEmpty(this.SelectedUser.Name) ? this.SelectedUser.Username : this.SelectedUser.Name;
                }

                string notifyPayload = $"{title}|{content}|{chatId}";

                // send notification
                _ = NotificationService.NotifyAsync(notifyPayload);

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (this.SelectedUser != null && this.SelectedUser.Id == chatId || this.SelectedUser != null && this.SelectedUser.UserUid == senderUid)
                        {
                            var msg = new OnesMessageModel
                            {
                                SenderUid = senderUid,
                                ChatId = chatId,
                                Content = content,
                                ContentType = contentType,
                                IsEdited = false,
                                EditContent = content,
                                DeliveredAt = DateTime.Now,
                                ReadAt = null,
                                IsMine = false,
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
    }
}
