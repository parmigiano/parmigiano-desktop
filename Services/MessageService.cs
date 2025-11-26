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
        private const int MaxMessageLength = 2500;

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

            // Убираем управляющие символы, кроме таба и перевода строки
            s = Regex.Replace(s, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", string.Empty);

            // Сжимаем многократные пробелы в один (включая табы/новые строки -> пробел)
            s = Regex.Replace(s, @"\s+", " ");

            if (s.Length > MaxMessageLength)
            {
                s = s.Substring(0, MaxMessageLength);
            }

            return s;
        }
    }
}
