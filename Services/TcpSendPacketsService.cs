using ClientRequestStruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Services
{
    public static class TcpSendPacketsService
    {
        #region PACKET UserStatus

        public static async Task SendOnlinePacketAsync(ulong uid, bool online)
        {
            var packet = new Request
            {
                RequestInfo = new RequestInfo
                {
                    Type = RequestInfo.Types.requestTypes.UserOnlineStatus,
                },
                ClientInfo = new ClientInfo
                {
                    Uid = uid,
                },
                ClientActivePacket = new ClientActivePacket
                {
                    Uid = uid,
                    Online = online,
                }
            };

            await ConnectionService.Instance.Tcp.SendProtoAsync(packet);
        }

        #endregion
    }
}
