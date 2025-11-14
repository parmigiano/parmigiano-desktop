using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class MessageModel
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("sender_uid")]
        public ulong SenderUid { get; set; }

        [JsonPropertyName("receiver_uid")]
        public ulong ReceiverUid { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("content_type")]
        public string ContentType { get; set; } = "text"; // text, image, video, file, etc.

        [JsonPropertyName("attachments")]
        public Dictionary<string, object>? Attachments { get; set; }

        [JsonPropertyName("is_edited")]
        public bool IsEdited { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("is_pinned")]
        public bool IsPinned { get; set; }
    }
}
