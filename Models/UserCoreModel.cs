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
        public ulong Id { get; set; }


        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }


        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }


        [JsonPropertyName("user_uid")]
        public ulong UserUid { get; set; }


        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("email_confirmed")]
        public bool EmailConfirmed { get; set; }


        [JsonPropertyName("password")]
        public string Password { get; set; }


        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }


        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
    }
}
