using Parmigiano.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Services
{
    public static class Logger
    {
        static Logger()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Config.Current.LOGS_PATH)!);
        }

        private static void Log(string level, string message, string fileName)
        {
            string folderName = DateTime.Now.ToString("dd.MM");
            string logFolder = Path.Combine(Config.Current.LOGS_PATH, folderName);

            Directory.CreateDirectory(logFolder);

            string logFilePath = Path.Combine(logFolder, fileName);

            string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

            try
            {
                File.AppendAllText(logFilePath, logLine + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
            }
        }

        public static void Info(string message)
        {
            Log("INFO", message, "Info.log");
        }

        public static void Error(string message)
        {
            Log("ERROR", message, "Error.log");
        }

        public static void Tcp(string message)
        {
            Log("TCP", message, "Tcp.log");
        }
    }
}
