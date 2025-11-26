using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Parmigiano.Models
{
    public class ReqEmailConfirmModel
    {
        [JsonPropertyName("email")]
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
