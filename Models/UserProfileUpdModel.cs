using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Parmigiano.Models
{
    public class UserProfileUpdModel
    {
        [JsonPropertyName("username")]
        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonPropertyName("name")]
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        [JsonProperty("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("overview")]
        [JsonProperty("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("username_visible")]
        [JsonProperty("username_visible", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UsernameVisible { get; set; }

        [JsonPropertyName("phone_visible")]
        [JsonProperty("phone_visible", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PhoneVisible { get; set; }

        [JsonPropertyName("email_visible")]
        [JsonProperty("email_visible", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EmailVisible { get; set; }

        [JsonPropertyName("password")]
        [JsonProperty("password")]
        public string? Password { get; set; }
    }
}
