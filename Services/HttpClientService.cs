using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Models;
using Parmigiano.Repository;
using Parmigiano.UI.Components;
using System;
using System.Collections.Concurrent;
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
        private readonly NetworkService _network = new NetworkService();
        private readonly IUserConfigRepository _userConf = new UserConfigRepository();

        private readonly ConcurrentQueue<DeferredRequestModel> _deferredRequests = new();
        private bool _isSendingDeferred = false;

        private static bool _isAuthWindowOpen = false;

        public HttpClientService(string baseUrl)
        {
            this._httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            string? authToken = this._userConf.GetString(UserConfigState.AUTH_SESSION_ID);

            if (authToken != null)
            {
                this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._userConf.GetString(UserConfigState.AUTH_SESSION_ID));
            }

            this._network.NetworkAvailabilityChanged += async () =>
            {
                if (this._network.IsAvailable)
                {
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await ProcessDeferredRequests();
                    });
                }
            };
        }

        #region Public API

        #region GET

        public async Task<T?> GetAsync<T>(string endpoint) where T : class
        {
            if (!this._network.IsAvailable)
            {
                EnqueueDeferred<T>(endpoint, HttpMethod.Get);
                this.ShowError("Нет интернета. Запрос отложен.");
                return default;
            }

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

        #endregion

        #region POST

        public async Task<T?> PostAsync<T>(string endpoint, object data) where T : class
        {
            if (!this._network.IsAvailable)
            {
                EnqueueDeferred<T>(endpoint, HttpMethod.Post, data);
                this.ShowError("Нет интернета. Запрос отложен.");
                return default;
            }

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

        #endregion

        #region PUT

        public async Task<T?> PutAsync<T>(string endpoint, object data) where T : class
        {
            if (!this._network.IsAvailable)
            {
                EnqueueDeferred<T>(endpoint, HttpMethod.Put, data);
                this.ShowError("Нет интернета. Запрос отложен.");
                return default;
            }

            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await this._httpClient.PutAsync(endpoint, content);
                return await HandleResponse<T>(response, "PUT", endpoint);
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

        #endregion

        #region PATCH

        public async Task<T?> PatchAsync<T>(string endpoint, object data) where T : class
        {
            if (!this._network.IsAvailable)
            {
                EnqueueDeferred<T>(endpoint, HttpMethod.Put, data);
                this.ShowError("Нет интернета. Запрос отложен.");
                return default;
            }

            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
                {
                    Content = content
                };

                var response = await this._httpClient.SendAsync(request);
                return await HandleResponse<T>(response, "PATCH", endpoint);
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

        #endregion

        #region DELETE

        public async Task<T?> DeleteAsync<T>(string endpoint) where T : class
        {
            if (!this._network.IsAvailable)
            {
                EnqueueDeferred<T>(endpoint, HttpMethod.Delete);
                this.ShowError("Нет интернета. Запрос отложен.");
                return default;
            }

            try
            {
                var response = await this._httpClient.DeleteAsync(endpoint);
                return await HandleResponse<T>(response, "DELETE", endpoint);
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

        #endregion

        #region UPLOAD

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

        #endregion

        #endregion

        #region Deferred Requests

        private void EnqueueDeferred<T>(string endpoint, HttpMethod method, object? data = null)
        {
            this._deferredRequests.Enqueue(new DeferredRequestModel
            {
                Endpoint = endpoint,
                Method = method,
                Data = data,
                ResponseType = typeof(T)
            });
        }

        #region ProcessDeferredRequests

        public async Task ProcessDeferredRequests()
        {
            if (this._isSendingDeferred) return;
            this._isSendingDeferred = true;

            while(this._deferredRequests.TryDequeue(out var request))
            {
                try
                {
                    if (!this._network.IsAvailable)
                    {
                        this._deferredRequests.Enqueue(request);
                        break;
                    }

                    switch (request.Method.Method)
                    {
                        case "GET":
                            {
                                var method = typeof(HttpClientService).GetMethod(nameof(GetAsync))!.MakeGenericMethod(request.ResponseType);
                                await (Task)method.Invoke(this, new object[] { request.Endpoint })!;
                                break;
                            }
                        case "POST":
                            {
                                if (request.Data is string filePath)
                                {
                                    await this.UploadFile(request.Endpoint, filePath);
                                }
                                else
                                {
                                    var method = typeof(HttpClientService).GetMethod(nameof(PostAsync))!.MakeGenericMethod(request.ResponseType);
                                    await (Task)method.Invoke(this, new object[] { request.Endpoint, request.Data! })!;
                                }

                                break;
                            }
                        case "PUT":
                            {
                                var method = typeof(HttpClientService).GetMethod(nameof(PutAsync))!.MakeGenericMethod(request.ResponseType);
                                await (Task)method.Invoke(this, new object[] { request.Endpoint, request.Data! })!;
                                break;
                            }
                        case "PATCH":
                            {
                                var method = typeof(HttpClientService).GetMethod(nameof(PatchAsync))!.MakeGenericMethod(request.ResponseType);
                                await (Task)method.Invoke(this, new object[] { request.Endpoint, request.Data! })!;
                                break;
                            }
                        case "DELETE":
                            {
                                var method = typeof(HttpClientService).GetMethod(nameof(DeleteAsync))!.MakeGenericMethod(request.ResponseType);
                                await (Task)method.Invoke(this, new object[] { request.Endpoint })!;
                                break;
                            }
                        default:
                            {
                                Logger.Error($"Unsupported HTTP method: {request.Method}");
                                break;
                            }
                    }
                }
                catch
                {
                    this._deferredRequests.Enqueue(request);
                }
            }

            this._isSendingDeferred = false;
        }

        #endregion

        #endregion

        #region Response Handling

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

            this._userConf.DeleteKey(UserConfigState.AUTH_SESSION_ID);

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

        #endregion

        private void ShowError(string message)
        {
            Notification.Show("Ошибка", message, NotificationType.Error);
        }
    }
}
