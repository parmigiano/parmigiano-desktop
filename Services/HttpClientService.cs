using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Parmigiano.Services
{
    public class HttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IUserConfigRepository _userConf = new UserConfigRepository();

        private static bool _isAuthWindowOpen = false;

        public HttpClientService(string baseUrl)
        {
            this._httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._userConf.Get("access_token"));
        }

        public async Task<T?> GetAsync<T>(string endpoint) where T : class
        {
            try
            {
                var response = await this._httpClient.GetAsync(endpoint);
                return await HandleResponse<T>(response, "GET", endpoint);
            }
            catch (HttpRequestException ex)
            {
                Logger.Error($"Network error: {ex.Message}");
                this.ShowError("Ошибка сети: нет соединения с сервером.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Unexpected error: {ex.Message}");
                this.ShowError($"Неизвестная ошибка: {ex.Message}");
            }

            return default;
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data) where T : class
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await this._httpClient.PostAsync(endpoint, content);
                return await HandleResponse<T>(response, "POST", endpoint);
            }
            catch (HttpRequestException ex)
            {
                Logger.Error($"Network error: {ex.Message}");
                this.ShowError("Ошибка сети: нет соединения с сервером.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Unexpected error: {ex.Message}");
                this.ShowError($"Неизвестная ошибка: {ex.Message}");
            }

            return default;
        }

        public async Task<string?> UploadFile(string endpoint, string filePath)
        {
            try
            {
                using var form = new MultipartFormDataContent();

                var stream = File.OpenRead(filePath);
                var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                form.Add(fileContent, "avatar", Path.GetFileName(filePath));

                var response = await this._httpClient.PostAsync(endpoint, form);
                var text = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"Ошибка загрузки: {response.StatusCode} — {text}");
                    ShowError($"Ошибка загрузки ({response.StatusCode})");
                    return null;
                }

                using var doc = JsonDocument.Parse(text);
                if (doc.RootElement.TryGetProperty("message", out var msgProp))
                {
                    return msgProp.GetString();
                }

                return doc.RootElement.GetProperty("url").GetString();
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка загрузки: {ex.Message}");
                ShowError($"Ошибка загрузки ({ex.Message})");
                return null;
            }
        }

        public async Task<T?> HandleResponse<T>(HttpResponseMessage response, string method, string endpoint) where T : class
        {
            var text = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Logger.Error($"[{method}] {endpoint} -> 401 Unauthorized");

                    this.HandleUnauthorized();

                    return default;
                }

                string? message = ExtractErrorMessage(text);

                Logger.Error($"[{method}] {endpoint} -> {response.StatusCode}: {message}");
                this.ShowError(message ?? $"Ошибка {response.StatusCode}");

                return default;
            }

            Logger.Info($"[{method}] {endpoint} -> OK ({response.StatusCode})");

            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponseModel<T>>(text, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return apiResponse?.Message;
            }
            catch (Exception ex)
            {
                Logger.Error($"Deserialize error: {ex.Message}");
                this.ShowError("Ошибка при разборе ответа сервера.");

                return default;
            }
        }

        private string? ExtractErrorMessage(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);

                if (doc.RootElement.TryGetProperty("message", out var msg))
                {
                    return msg.GetString();
                }
            }
            catch
            {
                // если не JSON, просто вернуть текст как есть
            }

            if (!string.IsNullOrWhiteSpace(responseBody))
            {
                return responseBody.Trim();
            }

            return null;
        }

        private void HandleUnauthorized()
        {
            if (_isAuthWindowOpen) return;
            _isAuthWindowOpen = true;

            this._userConf.DeleteKey("access_token");

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is MainWindow)
                    {
                        window.Close();
                        break;
                    }
                }

                var authWindow = new AuthWindow();

                authWindow.Closed += (_, _) => _isAuthWindowOpen = false;
                authWindow.Show();
            });
        }

        private void ShowError(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }
    }
}
