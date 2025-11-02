using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class OnesMessageModel
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }


        [JsonPropertyName("sender_uid")]
        public ulong SenderUid { get; set; }


        [JsonPropertyName("receiver_uid")]
        public ulong ReceiverUid { get; set; }


        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;


        [JsonPropertyName("content_type")]
        public string ContentType { get; set; } = "text";


        [JsonPropertyName("is_edited")]
        public bool IsEdited { get; set; }


        [JsonPropertyName("is_pinned")]
        public bool IsPinned { get; set; }


        [JsonPropertyName("delivered_at")]
        public DateTime DeliveredAt { get; set; }


        [JsonPropertyName("read_at")]
        public DateTime? ReadAt { get; set; }


        [JsonPropertyName("edit_content")]
        public string? EditContent { get; set; }

        public bool IsMine { get; set; }
    }
}
