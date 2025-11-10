using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Parmigiano.Services
{
    public static class UpdateAppService
    {
        public static async Task <(bool isUpdateAvailable, string downloadUrl)> CheckForUpdateAsync(string currentVersion)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("ParmigianoChat");

                var response = await client.GetStringAsync("https://api.github.com/repos/parmigiano/parmigiano-desktop/releases/latest");

                var json = JObject.Parse(response);

                string latestTag = json["tag_name"]?.ToString();
                string latestVersion = latestTag.TrimStart('v');

                if (new Version(latestVersion) > new Version(currentVersion))
                {
                    string downloadUrl = json["assets"]?[0]?["browser_download_url"]?.ToString();
                    return (true, downloadUrl);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get update version: {ex.Message}");
            }

            return (false, null);
        }

        public static async Task DownloadAndUpdateAsync(string url)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), "ParmigianoChatDesktop_Update.exe");

            using (var client = new HttpClient())
            using (var stream = await client.GetStreamAsync(url))
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fs);
            }

            System.Diagnostics.Process.Start(tempFile);

            Application.Current.Shutdown();
        }
    }
}
