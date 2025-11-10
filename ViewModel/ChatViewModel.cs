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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

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
            this.SendMessageCommand = new RelayCommand(_ => this.SendMessage());
            this.EditMessageCommand = new RelayCommand(msg => this.EditMessage(msg));
            this.DeleteMessageCommand = new RelayCommand(msg => this.DeleteMessage(msg));
            this.CopyMessageCommand = new RelayCommand(msg => this.CopyMessage(msg));
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

        // COMMANDS
        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(this.MessageText))
            {
                return;
            }

            if (this.EditingMessage != null)
            {
                this.EditingMessage.EditContent = this.MessageText;
                this.EditingMessage.IsEdited = true;
                this.EditingMessage = null;
            }
            else
            {
                /// SEND TO TCP

                this.Messages.Add(new OnesMessageModel
                {
                    SenderUid = AppSession.CurrentUser.UserUid,
                    Content = this.MessageText,
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = this.MessageText,
                    DeliveredAt = DateTime.Now,
                    ReadAt = null,
                    IsMine = true,
                });
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
