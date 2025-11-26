using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class ChatUpdateBlockedModel
    {
        [JsonPropertyName("chat_id")]
        [JsonProperty("chat_id")]
        public ulong ChatId { get; set; }

        [JsonPropertyName("blocked")]
        [JsonProperty("blocked")]
        public bool Blocked { get; set; }
    }
}
