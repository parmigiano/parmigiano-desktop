using System;
using System.Text.Json.Serialization;

namespace Parmigiano.Models
{
    public class ChatSettingModel
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("chat_id")]
        public ulong ChatId { get; set; }

        [JsonPropertyName("custom_background")]
        public string? CustomBackground { get; set; }

        [JsonPropertyName("blocked")]
        public bool Blocked { get; set; }
    }
}
