using Newtonsoft.Json.Linq;
using Parmigiano.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parmigiano.Services
{
    public class TcpClientService
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;

        public event Action<string, JObject> OnEventReceived;
        public bool IsConnected => _tcpClient != null && _tcpClient.Connected;

        public async void Connect()
        {
            if (this.IsConnected)
            {
                Logger.Info("TcpClientService: уже подключено, пропускаем повторное подключение.");
                return;
            }

            try
            {
                this._tcpClient = new TcpClient();
                await this._tcpClient.ConnectAsync(Config.Current.TCP_SERVER_ADDR, Config.Current.TCP_SERVER_PORT);

                this._stream = this._tcpClient.GetStream();
                this._cts = new CancellationTokenSource();

                Logger.Tcp($"[INFO] TCP client connected to {Config.Current.TCP_SERVER_ADDR}:{Config.Current.TCP_SERVER_PORT}");

                _ = Task.Run(() => this.ListenAsync(this._cts.Token));
            }
            catch (Exception ex)
            {
                Logger.Tcp("[ERROR] Error TCP-connection: " + ex.Message);
            }
        }

        private async Task ListenAsync(CancellationToken token)
        {
            var buffer = new byte[8192];

            while (!token.IsCancellationRequested)
            {
                try
                {
                    int bytesRead = await this._stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    try
                    {
                        var json = JObject.Parse(message);
                        var evt = json["event"]?.ToString();
                        var data = (JObject)json["data"];

                        OnEventReceived?.Invoke(evt, data);
                    }
                    catch
                    {
                        Logger.Tcp($"[INFO] bytes: {message.Length}: {message}");
                    }
                }
                catch (IOException)
                {
                    Logger.Tcp("[INFO] TCP connection closed");
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Error("[ERROR] TCP Error: " + ex.Message);
                    break;
                }
            }

            this.Disconnect();
        }

        public async Task SendAsync(object data)
        {
            if (!IsConnected)
            {
                Logger.Info("TCP: попытка отправки при отсутствии соединения");
                return;
            }

            try
            {
                string json = data is string s ? s : Newtonsoft.Json.JsonConvert.SerializeObject(data);
                byte[] bytes = Encoding.UTF8.GetBytes(json + "\n");
                await _stream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                Logger.Tcp("Failed send to TCP: " + ex.Message);
            }
        }

        public void Disconnect()
        {
            try
            {
                this._cts?.Cancel();
                this._stream?.Close();
                this._tcpClient?.Close();

                Logger.Tcp("TCP closed");
            }
            catch (Exception ex)
            {
                Logger.Tcp("Failed to closed connection TCP: " + ex.Message);
            }
        }
    }
}
