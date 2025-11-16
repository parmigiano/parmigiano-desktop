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

                var url = "https://api.github.com/repos/parmigiano/parmigiano-desktop/releases/latest";
                using var resp = await client.GetAsync(url);
                var content = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    Logger.Error($"Update check failed: HTTP {(int)resp.StatusCode} - {resp.ReasonPhrase}. Body: {content}");
                    return (false, null);
                }

                var json = JObject.Parse(content);

                if (json["message"] != null)
                {
                    Logger.Error($"Update check response contains message: {json["message"]}");
                    return (false, null);
                }

                string latestTag = json["tag_name"]?.ToString();
                if (string.IsNullOrWhiteSpace(latestTag))
                {
                    Logger.Error("Update check: tag_name is missing in GitHub response.");
                    return (false, null);
                }

                string latestVersion = latestTag.TrimStart('v', 'V');

                if (!Version.TryParse(latestVersion, out var verLatest))
                {
                    Logger.Error($"Update check: cannot parse latest version '{latestVersion}' from tag '{latestTag}'. Full JSON: {content}");
                    return (false, null);
                }

                if (!Version.TryParse(currentVersion, out var verCurrent))
                {
                    Logger.Error($"Update check: cannot parse current version '{currentVersion}'.");
                }

                if (verLatest <= verCurrent)
                {
                    return (false, null);
                }

                var assets = json["assets"] as JArray;
                string downloadUrl = null;

                if (assets != null && assets.Count > 0)
                {
                    var exeAsset = assets
                        .Children<JObject>()
                        .FirstOrDefault(a => (a["browser_download_url"]?.ToString() ?? "").EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || (a["name"]?.ToString() ?? "").EndsWith(".exe", StringComparison.OrdinalIgnoreCase));

                    if (exeAsset != null)
                    {
                        downloadUrl = exeAsset["browser_download_url"]?.ToString();
                    }
                    else
                    {
                        var anyAsset = assets.Children<JObject>().FirstOrDefault(a => a["browser_download_url"] != null);
                        downloadUrl = anyAsset?["browser_download_url"]?.ToString();
                    }
                }
                else
                {
                    downloadUrl = json["assets_url"]?.ToString();
                    Logger.Error("Update check: assets array is empty. Full JSON: " + content);
                }

                if (string.IsNullOrWhiteSpace(downloadUrl))
                {
                    Logger.Error("Update check: cannot determine download URL from release JSON.");
                    return (true, null);
                }

                return (true, downloadUrl);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to get update version: {ex.Message}");
            }

            return (false, null);
        }

        public static async Task DownloadAndUpdateAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            string tempFile = Path.Combine(Path.GetTempPath(), "ParmigianoChatDesktop_Update.exe");

            try
            {
                using (var client = new HttpClient())
                using (var stream = await client.GetStreamAsync(url))
                using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fs);
                }

                System.Diagnostics.Process.Start(tempFile);

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Logger.Error("DownloadAndUpdateAsync failed: " + ex.Message);
            }
        }
    }
}
