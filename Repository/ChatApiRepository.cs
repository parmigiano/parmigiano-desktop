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
    public class ChatApiRepository : IChatApiRepository
    {
        private readonly HttpClientService _httpClient = new(Config.Current.HTTP_SERVER_ADDR);

        private readonly string _apiPath = "chats";

        public async Task<List<ChatMinimalWithLMessageModel>> GetChats()
        {
            return await this._httpClient.GetAsync<List<ChatMinimalWithLMessageModel>>($"{this._apiPath}");
        }

        public async Task<List<OnesMessageModel>> GetHistory(ulong senderUid)
        {
            return await this._httpClient.GetAsync<List<OnesMessageModel>>($"{this._apiPath}/history/{senderUid}");
        }
    }
}
