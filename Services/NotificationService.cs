using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Services
{
    public static class NotificationService
    {
        public static void StartNotification()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Core.Config.Current.BROKER_APP_NAME);

            bool isRunning = Process.GetProcessesByName(Core.Config.Current.BROKER_PROCESS_NAME).Any();

            if (!isRunning && File.Exists(path))
            {
                ProcessStartInfo psi = new()
                {
                    FileName = path,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(psi);
            }
        }

        public static async Task NotifyAsync(string message)
        {
            try
            {
                using (var pipe = new NamedPipeClientStream(".", Core.Config.Current.BROKER_PROCESS_NAME, PipeDirection.Out))
                {
                    await pipe.ConnectAsync(1000);
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    await pipe.WriteAsync(data, 0, data.Length);
                }
            }
            catch
            {

            }
        }
    }
}
