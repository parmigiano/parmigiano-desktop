using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.Services;
using Parmigiano.Services.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private void HandleTcpEvent(ResponseStruct.Response response)
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
                    await TcpSendPacketsService.SendEditMessageAsync(this.EditingMessage.ChatId, this.EditingMessage.Id, prepared);
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
            if (msg is OnesMessageModel message && message.IsMine)
            {
                this.EditingMessage = message;
            }
        }

        private void DeleteMessage(object msg)
        {
            if (msg is OnesMessageModel message && this.Messages.Contains(message) && message.IsMine)
            {
                this.Messages.Remove(message);
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
    }
}
