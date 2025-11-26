using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Services;
using Parmigiano.UI.Components;
using SharpCompress.Common;
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

        public async Task<string?> ChatUpdateBlocked(ChatUpdateBlockedModel chatBlocked)
        {
            return await this._httpClient.PatchAsync<string?>($"{this._apiPath}/{chatBlocked.ChatId}/blocked", chatBlocked);
        }

        public async Task<string> ChatUpdateCustomBackground(ulong chatId, string? filepath)
        {
            return await this._httpClient.UploadFile($"{this._apiPath}/{chatId}/cbackground", filepath, "background");
        }

        public async Task<List<ChatMinimalWithLMessageModel>> GetChats()
        {
            return await this._httpClient.GetAsync<List<ChatMinimalWithLMessageModel>>($"{this._apiPath}");
        }

        public async Task<ChatSettingModel?> GetChatSetting(ulong chatId)
        {
            return await this._httpClient.GetAsync<ChatSettingModel?>($"{this._apiPath}/{chatId}/settings");
        }

        public async Task<List<OnesMessageModel>> GetHistory(ulong senderUid)
        {
            return await this._httpClient.GetAsync<List<OnesMessageModel>>($"{this._apiPath}/history/{senderUid}");
        }
    }
}
