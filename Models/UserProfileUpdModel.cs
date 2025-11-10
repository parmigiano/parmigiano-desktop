using Newtonsoft.Json;

namespace Parmigiano.Models
{
    public class UserProfileUpdModel
    {
        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }
    }
}
