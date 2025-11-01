using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class MessageStatusModel
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }


        [JsonPropertyName("message_id")]
        public long MessageId { get; set; }


        [JsonPropertyName("receiver_uid")]
        public long ReceiverUid { get; set; }


        [JsonPropertyName("delivered_at")]
        public DateTime DeliveredAt { get; set; }


        [JsonPropertyName("read_at")]
        public DateTime? ReadAt { get; set; }
    }
}
