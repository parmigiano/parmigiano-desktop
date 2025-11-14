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
        public ulong Id { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("user_uid")]
        public ulong UserUid { get; set; }

        [JsonPropertyName("online")]
        public bool Online { get; set; }

        [JsonPropertyName("last_online_date")]
        public DateTime LastOnlineDate { get; set; }

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("username_visible")]
        public bool UsernameVisible { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("email_visible")]
        public bool EmailVisible { get; set; }

        [JsonPropertyName("email_confirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("phone_visible")]
        public bool PhoneVisible { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }
    }
}
