using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class UserCoreModel
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

        [JsonPropertyName("email")]
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonPropertyName("email_confirmed")]
        [JsonProperty("email_confirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonPropertyName("password")]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
