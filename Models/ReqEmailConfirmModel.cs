using System.Text.Json.Serialization;

namespace Parmigiano.Models
{
    public class ReqEmailConfirmModel
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
