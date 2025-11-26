using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class UserInfoModel
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("created_at")]
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("user_uid")]
        [JsonProperty("user_uid")]
        public ulong UserUid { get; set; }

        [JsonPropertyName("online")]
        [JsonProperty("online")]
        public bool Online { get; set; }

        [JsonPropertyName("last_online_date")]
        [JsonProperty("last_online_date")]
        public DateTime LastOnlineDate { get; set; }

        [JsonPropertyName("avatar")]
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("name")]
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        [JsonProperty("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("username_visible")]
        [JsonProperty("username_visible")]
        public bool UsernameVisible { get; set; }

        [JsonPropertyName("email")]
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("email_visible")]
        [JsonProperty("email_visible")]
        public bool EmailVisible { get; set; }

        [JsonPropertyName("email_confirmed")]
        [JsonProperty("email_confirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonPropertyName("phone")]
        [JsonProperty("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("phone_visible")]
        [JsonProperty("phone_visible")]
        public bool PhoneVisible { get; set; }

        [JsonPropertyName("overview")]
        [JsonProperty("overview")]
        public string? Overview { get; set; }
    }
}
