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
        Task<List<OnesMessageModel>?> GetHistory(ulong senderUid);

        Task<List<ChatMinimalWithLMessageModel>?> GetChats();
    }
}
