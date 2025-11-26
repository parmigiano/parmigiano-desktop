using Newtonsoft.Json;
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
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("created_at")]
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("deleted_at")]
        [JsonProperty("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("sender_uid")]
        [JsonProperty("sender_uid")]
        public ulong SenderUid { get; set; }

        [JsonPropertyName("receiver_uid")]
        [JsonProperty("receiver_uid")]
        public ulong ReceiverUid { get; set; }

        [JsonPropertyName("content")]
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonPropertyName("content_type")]
        [JsonProperty("content_type")]
        public string ContentType { get; set; } = "text"; // text, image, video, file, etc.

        [JsonPropertyName("attachments")]
        [JsonProperty("attachments")]
        public Dictionary<string, object>? Attachments { get; set; }

        [JsonPropertyName("is_edited")]
        [JsonProperty("is_edited")]
        public bool IsEdited { get; set; }

        [JsonPropertyName("is_deleted")]
        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("is_pinned")]
        [JsonProperty("is_pinned")]
        public bool IsPinned { get; set; }
    }
}
