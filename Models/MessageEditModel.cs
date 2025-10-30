using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class MessageEditModel
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }


        [JsonPropertyName("message_id")]
        public long MessageId { get; set; }


        [JsonPropertyName("old_content")]
        public string? OldContent { get; set; }


        [JsonPropertyName("new_content")]
        public string NewContent { get; set; }


        [JsonPropertyName("editor_uuid")]
        public string? EditorUUID { get; set; }


        [JsonPropertyName("edited_at")]
        public DateTime EditedAt { get; set; }
    }
}
