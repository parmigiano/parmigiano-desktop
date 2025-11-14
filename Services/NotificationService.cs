using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Services
{
    public static class NotificationService
    {
        public static void StartNotification()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Core.Config.Current.BROKER_APP_NAME);

            string brokerProcessName = Path.GetFileNameWithoutExtension(Core.Config.Current.BROKER_APP_NAME);

            var matchingProcesses = new List<Process>();
            foreach (var process in Process.GetProcessesByName(brokerProcessName))
            {
                try
                {
                    string exePath = process.MainModule?.FileName;
                    if (!string.IsNullOrEmpty(exePath) && string.Equals(Path.GetFullPath(exePath), Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase))
                    {
                        matchingProcesses.Add(process);
                    }
                }
                catch
                {
                }
            }

            if (matchingProcesses.Count > 1)
            {
                var ordered = matchingProcesses.OrderBy(p =>
                {
                    try
                    {
                        return p.StartTime;
                    }
                    catch
                    {
                        return DateTime.MaxValue;
                    }
                }).ToList();

                for (int i = 1; i < ordered.Count; i++)
                {
                    try
                    {
                        ordered[i].CloseMainWindow();
                        if (!ordered[i].WaitForExit(1000))
                        {
                            ordered[i].Kill();
                        }
                    }
                    catch
                    {
                        try
                        {
                            ordered[i].Kill();
                        }
                        catch
                        {
                        }
                    }
                }
            }

            bool isRunning = matchingProcesses.Any();

            if (!isRunning && File.Exists(path))
            {
                ProcessStartInfo psi = new()
                {
                    FileName = path,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                try
                {
                    Process.Start(psi);
                }
                catch
                {
                }
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
