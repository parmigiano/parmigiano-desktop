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

        private static void Log(string level, string message)
        {
            string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            try
            {
                File.AppendAllText(Config.Current.LOGS_PATH, logLine + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
            }
        }

        public static void Info(string message)
        {
            Log("INFO", message);
        }

        public static void Error(string message)
        {
            Log("ERROR", message);
        }
    }
}
