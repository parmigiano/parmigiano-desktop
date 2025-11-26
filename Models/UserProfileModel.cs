using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class UserProfileModel
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

        [JsonPropertyName("user_uid")]
        [JsonProperty("user_uid")]
        public ulong UserUid { get; set; }

        [JsonPropertyName("avatar")]
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonPropertyName("username")]
        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
