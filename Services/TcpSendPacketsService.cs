using ClientRequestStruct;
using Parmigiano.Core;
using System;
using System.Threading.Tasks;

namespace Parmigiano.Services
{
    public static class TcpSendPacketsService
    {
        private static Request CreateBase(RequestInfo.Types.requestTypes type)
        {
            var uid = AppSession.CurrentUser?.UserUid ?? 0UL;

            return new Request
            {
                RequestInfo = new RequestInfo
                {
                    Type = type,
                },
                ClientInfo = new ClientInfo
                {
                    Uid = uid,
                }
            };
        }

        #region PACKET UserStatus

        public static async Task SendOnlinePacketAsync(ulong uid, bool online)
        {
            if (!ConnectionService.Instance.IsConnectedTcp)
            {
                Logger.Tcp("SendOnlinePacketAsync: TCP not connected");
                return;
            }

            var req = CreateBase(RequestInfo.Types.requestTypes.UserOnlineStatus);
            req.ClientActivePacket = new ClientActivePacket
            {
                Uid = uid,
                Online = online,
            };

            await SafeSendAsync(req);
        }

        #endregion

        #region PACKET SendMessage

        public static async Task SendMessageAsync(ulong chatId, string content, string contentType = "text")
        {
            if (!ConnectionService.Instance.IsConnectedTcp)
            {
                Logger.Tcp("SendMessageAsync: TCP not connected");
                return;
            }

            var uid = AppSession.CurrentUser?.UserUid ?? 0UL;

            var req = CreateBase(RequestInfo.Types.requestTypes.SendMessage);
            req.ClientSendMessagePacket = new ClientSendMessagePacket
            {
                Uid = uid,
                ChatId = chatId,
                Content = content ?? string.Empty,
                ContentType = contentType ?? "text",
            };

            await SafeSendAsync(req);
        }

        #endregion

        #region PACKET Typing

        public static async Task SendTypingAsync(ulong chatId, bool isTyping)
        {
            if (!ConnectionService.Instance.IsConnectedTcp)
            {
                Logger.Tcp("SendTypingAsync: TCP not connected");
                return;
            }

            var uid = AppSession.CurrentUser?.UserUid ?? 0UL;

            var req = CreateBase(RequestInfo.Types.requestTypes.UserTyping);
            req.ClientTypingPacket = new ClientTypingPacket
            {
                Uid = uid,
                ChatId = chatId,
                IsTyping = isTyping,
            };

            await SafeSendAsync(req);
        }

        #endregion

        #region PACKET ReadMessage

        public static async Task SendReadMessageAsync(ulong chatId, ulong messageId)
        {
            if (!ConnectionService.Instance.IsConnectedTcp)
            {
                Logger.Tcp("SendReadMessageAsync: TCP not connected");
                return;
            }

            var uid = AppSession.CurrentUser?.UserUid ?? 0UL;

            var req = CreateBase(RequestInfo.Types.requestTypes.ReadMessage);
            req.ClientReadMessagePacket = new ClientReadMessagePacket
            {
                Uid = uid,
                ChatId = chatId,
                MessageId = messageId
            };

            await SafeSendAsync(req);
        }

        #endregion

        #region PACKET EditMessage

        public static async Task SendEditMessageAsync(ulong chatId, ulong messageId, string newContent)
        {
            if (!ConnectionService.Instance.IsConnectedTcp)
            {
                Logger.Tcp("SendEditMessageAsync: TCP not connected");
                return;
            }

            var uid = AppSession.CurrentUser?.UserUid ?? 0UL;

            var req = CreateBase(RequestInfo.Types.requestTypes.EditMessage);
            req.ClientEditMessagePacket = new ClientEditMessagePacket
            {
                Uid = uid,
                ChatId = chatId,
                MessageId = messageId,
                Content = newContent ?? string.Empty,
            };

            await SafeSendAsync(req);
        }

        #endregion

        #region PACKET DeleteMessage

        public static async Task SendDeleteMessageAsync(ulong chatId, ulong messageId)
        {
            if (!ConnectionService.Instance.IsConnectedTcp)
            {
                Logger.Tcp("SendDeleteMessageAsync: TCP not connected");
                return;
            }

            var uid = AppSession.CurrentUser?.UserUid ?? 0UL;

            var req = CreateBase(RequestInfo.Types.requestTypes.DeleteMessage);
            req.ClientDeleteMessagePacket = new ClientDeleteMessagePacket
            {
                Uid = uid,
                ChatId = chatId,
                MessageId = messageId,
            };

            await SafeSendAsync(req);
        }

        #endregion

        #region PACKET PinMessage

        public static async Task SendPinMessageAsync(ulong chatId, ulong messageId)
        {
            if (!ConnectionService.Instance.IsConnectedTcp)
            {
                Logger.Tcp("SendPinMessageAsync: TCP not connected");
                return;
            }

            var uid = AppSession.CurrentUser?.UserUid ?? 0UL;

            var req = CreateBase(RequestInfo.Types.requestTypes.PinMessage);

            await SafeSendAsync(req);
        }

        #endregion

        private static async Task SafeSendAsync(Request req)
        {
            try
            {
                await ConnectionService.Instance.Tcp.SendProtoAsync(req);
            }
            catch (Exception ex)
            {
                Logger.Tcp($"SafeSendAsync: failed to send request: {ex.Message}");
            }
        }
    }
}
