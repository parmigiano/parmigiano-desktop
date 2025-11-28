using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parmigiano.Services
{
    public static class MessageService
    {
        public static int MaxMessageLength = 3000;
        public static int MaxMessagePartLength = 700;

        public static async Task SendMessageAsync(ulong chatId, ulong tmpMessageId, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            string prepared = PreprocessMessage(message);

            if (string.IsNullOrEmpty(prepared)) return;

            try
            {
                await TcpSendPacketsService.SendMessageAsync(chatId, tmpMessageId, prepared, "text");
                Logger.Info($"SendMessageAsync: message sent to chat {chatId}, len={prepared.Length}");
            }
            catch (Exception ex)
            {
                Logger.Error("MessageService.SendMessageAsync failed: " + ex.Message);
            }
        }

        public static string PreprocessMessage(string message)
        {
            if (message == null) return string.Empty;

            string s = message.Trim();

            s = Regex.Replace(s, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", "");

            s = Regex.Replace(s, @"[ \t]+", " ");

            s = Regex.Replace(s, @"^[ ]+", "", RegexOptions.Multiline);
            s = Regex.Replace(s, @"[ ]+$", "", RegexOptions.Multiline);

            if (s.Length > MaxMessageLength)
            {
                s = s.Substring(0, MaxMessageLength);
            }

            return s;
        }
    }
}
