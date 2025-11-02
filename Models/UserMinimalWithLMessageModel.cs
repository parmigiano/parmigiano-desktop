using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class UserMinimalWithLMessageModel : INotifyPropertyChanged
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }


        [JsonPropertyName("username")]
        public string Username { get; set; }


        [JsonPropertyName("avatar")]
        private string? _avatar;
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
        public ulong UserUid { get; set; }

        [JsonPropertyName("online")]
        public bool Online { get; set; }

        [JsonPropertyName("last_online_date")]
        public DateTime? LastOnlineDate { get; set; }


        [JsonPropertyName("last_message")]
        public string? LastMessage { get; set; }


        [JsonPropertyName("last_message_date")]
        public DateTime? LastMessageDate { get; set; }

        [JsonPropertyName("unread_message_count")]
        public short UnreadMessageCount { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
