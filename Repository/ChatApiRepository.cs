using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Services;
using Parmigiano.UI.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parmigiano.Repository
{
    public class ChatApiRepository : IChatApiRepository
    {
        private readonly HttpClientService _httpClient = new(Config.Current.HTTP_SERVER_ADDR);

        private readonly string _apiPath = "chats";

        public async Task<string?> ChatUpdateBlocked(ChatUpdateBlockedModel chatBlocked)
        {
            return await this._httpClient.PatchAsync<string?>($"{this._apiPath}/{chatBlocked.ChatId}/s/blocked", chatBlocked);
        }

        public async Task<string> ChatUpdateCustomBackground(ulong chatId, string? filepath)
        {
            return await this._httpClient.UploadFile($"{this._apiPath}/{chatId}/s/cbackground", filepath, "background");
        }

        public async Task<List<OnesMessageModel>> GetChannelChatHistory(ulong chatId, int? offset)
        {
            return await this._httpClient.GetAsync<List<OnesMessageModel>?>($"{this._apiPath}/channel/{chatId}/history?offset={offset}");
        }

        public async Task<List<ChatMinimalWithLMessageModel>> GetChats()
        {
            return await this._httpClient.GetAsync<List<ChatMinimalWithLMessageModel>>($"{this._apiPath}");
        }

        public async Task<List<ChatMinimalWithLMessageModel>?> GetChatsByUsername(string username)
        {
            return await this._httpClient.GetAsync<List<ChatMinimalWithLMessageModel>>($"{this._apiPath}?username={username}");
        }

        public async Task<ChatSettingModel?> GetChatSetting(ulong chatId)
        {
            return await this._httpClient.GetAsync<ChatSettingModel?>($"{this._apiPath}/{chatId}/s");
        }

        public async Task<List<OnesMessageModel>> GetGroupChatHistory(ulong chatId, int? offset)
        {
            return await this._httpClient.GetAsync<List<OnesMessageModel>?>($"{this._apiPath}/group/{chatId}/history?offset={offset}");
        }

        public async Task<List<OnesMessageModel>> GetPrivateChatHistory(ulong companionUid, int? offset)
        {
            return await this._httpClient.GetAsync<List<OnesMessageModel>?>($"{this._apiPath}/private/{companionUid}/history?offset={offset}");
        }
    }
}
