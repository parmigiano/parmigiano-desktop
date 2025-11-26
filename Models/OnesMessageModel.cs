using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class OnesMessageModel : INotifyPropertyChanged
    {
        private ulong _id;

        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public ulong Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        private ulong _chatId;

        [JsonPropertyName("chat_id")]
        [JsonProperty("chat_id")]
        public ulong ChatId
        {
            get => _chatId;
            set => Set(ref _chatId, value);
        }

        private ulong _senderUid;

        [JsonPropertyName("sender_uid")]
        [JsonProperty("sender_uid")]
        public ulong SenderUid
        {
            get => _senderUid;
            set => Set(ref _senderUid, value);
        }

        private string _content;

        [JsonPropertyName("content")]
        [JsonProperty("content")]
        public string Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        private string _contentType;

        [JsonPropertyName("content_type")]
        [JsonProperty("content_type")]
        public string ContentType
        {
            get => _contentType;
            set => Set(ref _contentType, value);
        }

        private bool _isEdited;

        [JsonPropertyName("is_edited")]
        [JsonProperty("is_edited")]
        public bool IsEdited
        {
            get => _isEdited;
            set => Set(ref _isEdited, value);
        }

        private bool _isPinned;

        [JsonPropertyName("is_pinned")]
        [JsonProperty("is_pinned")]
        public bool IsPinned
        {
            get => _isPinned;
            set => Set(ref _isPinned, value);
        }

        private DateTime? _deliveredAt;

        [JsonPropertyName("delivered_at")]
        [JsonProperty("delivered_at")]
        public DateTime? DeliveredAt
        {
            get => _deliveredAt;
            set => Set(ref _deliveredAt, value);
        }

        private DateTime? _readAt;

        [JsonPropertyName("read_at")]
        [JsonProperty("read_at")]
        public DateTime? ReadAt
        {
            get => _readAt;
            set => Set(ref _readAt, value);
        }

        private string? _editContent;

        [JsonPropertyName("edit_content")]
        [JsonProperty("edit_content")]
        public string? EditContent
        {
            get => _editContent;
            set => Set(ref _editContent, value);
        }

        private bool _isMine;
        public bool IsMine
        {
            get => _isMine;
            set => Set(ref _isMine, value);
        }


        // notify
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(name);

            return true;
        }
    }
}
