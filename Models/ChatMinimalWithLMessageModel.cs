using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class ChatMinimalWithLMessageModel : INotifyPropertyChanged
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonPropertyName("username")]
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        [JsonProperty("email")]
        public string Email { get; set; }

        private bool _email_visible;
        [JsonPropertyName("email_visible")]
        [JsonProperty("email_visible")]
        public bool EmailVisible
        {
            get => _email_visible;
            set
            {
                if (_email_visible != value)
                {
                    _email_visible = value;
                    OnPropertyChanged(nameof(EmailVisible));
                }
            }
        }

        private string? _avatar;
        [JsonPropertyName("avatar")]
        [JsonProperty("avatar")]
        public string? Avatar
        {
            get => _avatar;
            set
            {
                if (_avatar != value)
                {
                    _avatar = value;
                    OnPropertyChanged(nameof(Avatar));
                }
            }
        }

        [JsonPropertyName("user_uid")]
        [JsonProperty("user_uid")]
        public ulong UserUid { get; set; }

        private bool _online;
        [JsonPropertyName("online")]
        [JsonProperty("online")]
        public bool Online
        {
            get => _online;
            set
            {
                _online = value;
                OnPropertyChanged(nameof(Online));
            }
        }

        private DateTime? _last_online_date;
        [JsonPropertyName("last_online_date")]
        [JsonProperty("last_online_date")]
        public DateTime? LastOnlineDate
        {
            get => _last_online_date;
            set
            {
                _last_online_date = value;
                OnPropertyChanged(nameof(LastOnlineDate));
            }
        }

        private string _last_message;
        [JsonPropertyName("last_message")]
        [JsonProperty("last_message")]
        public string LastMessage
        {
            get => _last_message;
            set
            {
                _last_message = value;
                OnPropertyChanged(nameof(LastMessage));
            }
        }

        private DateTime? _last_message_date;
        [JsonPropertyName("last_message_date")]
        [JsonProperty("last_message_date")]
        public DateTime? LastMessageDate
        {
            get => _last_message_date;
            set
            {
                _last_message_date = value;
                OnPropertyChanged(nameof(LastMessageDate));
            }
        }

        private short _unread_message_count;
        [JsonPropertyName("unread_message_count")]
        [JsonProperty("unread_message_count")]
        public short UnreadMessageCount
        {
            get => _unread_message_count;
            set
            {
                _unread_message_count = value;
                OnPropertyChanged(nameof(UnreadMessageCount));
            }
        }

        private bool _isTyping;
        public bool IsTyping
        {
            get => _isTyping;
            set
            {
                _isTyping = value;
                OnPropertyChanged(nameof(IsTyping));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
