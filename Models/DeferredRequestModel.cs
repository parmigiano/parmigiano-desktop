using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Models
{
    public class DeferredRequestModel
    {
        public string Endpoint { get; set; } = string.Empty;

        public HttpMethod Method { get; set; }

        public object? Data { get; set; }

        public Type ResponseType { get; set; } = typeof(object);

        public TaskCompletionSource<object?> Completion { get; set; } = new();
    }
}
