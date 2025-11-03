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
        [JsonPropertyName("id")]
        private ulong _id;
        public ulong Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        [JsonPropertyName("sender_uid")]
        private ulong _senderUid;
        public ulong SenderUid
        {
            get => _senderUid;
            set => Set(ref _senderUid, value);
        }

        [JsonPropertyName("receiver_uid")]
        private ulong _receiverUid;
        public ulong ReceiverUid
        {
            get => _receiverUid;
            set => Set(ref _receiverUid, value);
        }

        [JsonPropertyName("content")]
        private string _content;
        public string Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        [JsonPropertyName("content_type")]
        private string _contentType;
        public string ContentType
        {
            get => _contentType;
            set => Set(ref _contentType, value);
        }


        [JsonPropertyName("is_edited")]
        private bool _isEdited;
        public bool IsEdited
        {
            get => _isEdited;
            set => Set(ref _isEdited, value);
        }

        [JsonPropertyName("is_pinned")]
        private bool _isPinned;
        public bool IsPinned
        {
            get => _isPinned;
            set => Set(ref _isPinned, value);
        }


        [JsonPropertyName("delivered_at")]
        private DateTime? _deliveredAt;
        public DateTime? DeliveredAt
        {
            get => _deliveredAt;
            set => Set(ref _deliveredAt, value);
        }

        [JsonPropertyName("read_at")]
        private DateTime? _readAt;
        public DateTime? ReadAt
        {
            get => _readAt;
            set => Set(ref _readAt, value);
        }

        [JsonPropertyName("edit_content")]
        private string _editContent;
        public string EditContent
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
