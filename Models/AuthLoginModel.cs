using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class AuthLoginModel
    {
        [JsonPropertyName("email")]
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;
    }
}
