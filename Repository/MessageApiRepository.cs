using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Repository
{
    public class MessageApiRepository : IMessageApiRepository
    {
        private readonly HttpClientService _httpClient = new(Config.Current.HTTP_SERVER_ADDR);

        private readonly string _apiPath = "messages";
    }
}
