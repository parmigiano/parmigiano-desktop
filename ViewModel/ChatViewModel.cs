using Parmigiano.Core;
using Parmigiano.Models;
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
using System.Windows.Input;

namespace Parmigiano.ViewModel
{
    public class ChatViewModel : BaseView
    {
        public ICommand SendMessageCommand { get; }
        public ICommand EditMessageCommand { get; }
        public ICommand DeleteMessageCommand { get; }

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

                this.MessageText = this._editingMessage?.Content;
            }
        }

        public ObservableCollection<OnesMessageModel> Messages { get; set; } = new();

        private UserMinimalWithLMessageModel _selectedUser;
        public UserMinimalWithLMessageModel SelectedUser
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
            this.EditMessageCommand = new RelayCommand(msg => this.StartEdit(msg));
            this.DeleteMessageCommand = new RelayCommand(msg => this.DeleteMessage(msg));
        }

        private async void LoadMessagesAsync()
        {
            if (this.SelectedUser == null)
            {
                return;
            }

            this.Messages.Clear();

            var now = DateTime.Now;

            var messages = new[]
            {
                new OnesMessageModel
                {
                    Id = 1,
                    SenderUid = AppSession.CurrentUserUid,
                    ReceiverUid = SelectedUser.UserUid,
                    Content = "Привет! Как дела?",
                    ContentType = "text",
                    IsEdited = false,
                    DeliveredAt = now.AddHours(-25), // вчера
                    ReadAt = now.AddHours(-24.5),
                },
                new OnesMessageModel
                {
                    Id = 2,
                    SenderUid = SelectedUser.UserUid,
                    ReceiverUid = AppSession.CurrentUserUid,
                    Content = "Привет Всё отлично! А ты как?",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddHours(-24),
                    ReadAt = now.AddHours(-23.9),
                },
                new OnesMessageModel
                {
                    Id = 3,
                    SenderUid = AppSession.CurrentUserUid,
                    ReceiverUid = SelectedUser.UserUid,
                    Content = "Я тоже отлично, на работе как всегда",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddHours(-23.5),
                    ReadAt = now.AddHours(-23.4),
                },
                new OnesMessageModel
                {
                    Id = 4,
                    SenderUid = SelectedUser.UserUid,
                    ReceiverUid = AppSession.CurrentUserUid,
                    Content = "А у нас сегодня снег пошёл",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddHours(-23.3),
                    ReadAt = now.AddHours(-23.1),
                },
                new OnesMessageModel
                {
                    Id = 5,
                    SenderUid = AppSession.CurrentUserUid,
                    ReceiverUid = SelectedUser.UserUid,
                    Content = "Ого, уже зима подкралась",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddHours(-23),
                    ReadAt = now.AddHours(-22.9),
                },
                new OnesMessageModel
                {
                    Id = 6,
                    SenderUid = SelectedUser.UserUid,
                    ReceiverUid = AppSession.CurrentUserUid,
                    Content = "Да! А у тебя там как погода?",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddMinutes(-10),
                    ReadAt = now.AddMinutes(-9.8),
                },
                new OnesMessageModel
                {
                    Id = 7,
                    SenderUid = AppSession.CurrentUserUid,
                    ReceiverUid = SelectedUser.UserUid,
                    Content = "Пока ещё тепло",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddMinutes(-5),
                    ReadAt = now.AddMinutes(-4.5),
                },
                new OnesMessageModel
                {
                    Id = 8,
                    SenderUid = SelectedUser.UserUid,
                    ReceiverUid = AppSession.CurrentUserUid,
                    Content = "Класс!",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddMinutes(-2),
                    ReadAt = null // ещё не прочитано
                },
                new OnesMessageModel
                {
                    Id = 9,
                    SenderUid = AppSession.CurrentUserUid,
                    ReceiverUid = SelectedUser.UserUid,
                    Content = "Кстати, видел новый фильм?",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddSeconds(-20),
                    ReadAt = null // тоже не прочитано
                },
                new OnesMessageModel
                {
                    Id = 10,
                    SenderUid = AppSession.CurrentUserUid,
                    ReceiverUid = SelectedUser.UserUid,
                    Content = "Тот самый, про астронавтов",
                    ContentType = "text",
                    IsEdited = true,
                    EditContent = "Тот самый",
                    DeliveredAt = now.AddSeconds(-19), // разница 1 секунда
                    ReadAt = null
                },
                new OnesMessageModel
                {
                    Id = 11,
                    SenderUid = SelectedUser.UserUid,
                    ReceiverUid = AppSession.CurrentUserUid,
                    Content = "О да, я смотрел его месяц назад!",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddMonths(-1),
                    ReadAt = now.AddMonths(-1).AddMinutes(10)
                },
                new OnesMessageModel
                {
                    Id = 12,
                    SenderUid = SelectedUser.UserUid,
                    ReceiverUid = AppSession.CurrentUserUid,
                    Content = "О да, я смотрел!",
                    ContentType = "text",
                    IsEdited = true,
                    EditContent = "да, я смотрел",
                    DeliveredAt = now.AddMonths(-1),
                    ReadAt = now.AddMonths(-1).AddMinutes(10)
                },
                 new OnesMessageModel
                {
                    Id = 13,
                    SenderUid = AppSession.CurrentUserUid,
                    ReceiverUid = SelectedUser.UserUid,
                    Content = "О да, я смотрел",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddMonths(-1),
                    ReadAt = null
                },
                  new OnesMessageModel
                {
                    Id = 14,
                    SenderUid = AppSession.CurrentUserUid,
                    ReceiverUid = SelectedUser.UserUid,
                    Content = "О да, я смотрел, это было чтото с чем. Короче это просто вау эффект. Ну момент конечно был хреновый такой но блять это АХУЕНО!",
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = now.AddMonths(-1),
                    ReadAt = null
                }
            };

            foreach (var message in messages)
            {
                message.IsMine = message.SenderUid == AppSession.CurrentUserUid;
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
                this.EditingMessage.Content = this.MessageText;
                this.EditingMessage.IsEdited = true;
                this.EditingMessage = null;
            }
            else
            {
                /// SEND TO TCP

                this.Messages.Add(new OnesMessageModel
                {
                    SenderUid = AppSession.CurrentUserUid,
                    ReceiverUid = SelectedUser.UserUid,
                    Content = this.MessageText,
                    ContentType = "text",
                    IsEdited = false,
                    EditContent = null,
                    DeliveredAt = DateTime.Now,
                    ReadAt = null,
                    IsMine = true,
                });
            }

            this.MessageText = string.Empty;
        }

        private void StartEdit(object msg)
        {
            if (msg is OnesMessageModel message)
            {
                this.EditingMessage = message;
            }
        }

        private void DeleteMessage(object msg)
        {
            if (msg is OnesMessageModel message && this.Messages.Contains(message))
            {
                this.Messages.Remove(message);
            }
        }
    }
}
