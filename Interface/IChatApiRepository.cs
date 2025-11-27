using Parmigiano.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Interface
{
    public interface IChatApiRepository
    {
        Task<List<ChatMinimalWithLMessageModel>?> GetChatsByUsername(string username);

        Task<List<ChatMinimalWithLMessageModel>?> GetChats();

        Task<ChatSettingModel?> GetChatSetting(ulong chatId);

        Task<List<OnesMessageModel>?> GetPrivateChatHistory(ulong companionUid, int? offset);

        Task<List<OnesMessageModel>?> GetGroupChatHistory(ulong chatId, int? offset);

        Task<List<OnesMessageModel>?> GetChannelChatHistory(ulong chatId, int? offset);

        Task<string?> ChatUpdateBlocked(ChatUpdateBlockedModel chatBlocked);

        Task<string?> ChatUpdateCustomBackground(ulong chatId, string? filepath);
    }
}
