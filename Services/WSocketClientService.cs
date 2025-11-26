using Newtonsoft.Json.Linq;
using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Parmigiano.Services
{
    public class WSocketClientService
    {
        private WebSocket _wsocket;
        public event Action<string, JObject> OnEventReceived;

        // timers
        private readonly object _lock = new();
        private bool _manualClose = false;
        private Timer _reconnectTimer;

        public bool IsConnected => this._wsocket != null && this._wsocket.IsAlive;

        public void Connect(ulong userUid, string token = null)
        {
            lock (this._lock)
            {
                if (IsConnected)
                {
                    Logger.Info("WSocketClientService: уже подключено, пропускаем повторное подключение.");
                    return;
                }

                this._manualClose = false;
                string url = $"{Config.Current.WSOCKET_SERVER_ADDR}/wsocket?uid={userUid}";

                this._wsocket = new WebSocket(url);

                // tls
                this._wsocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                this._wsocket.SslConfiguration.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;


                this._wsocket.OnOpen += (s, e) =>
                {
                    Logger.Info($"WebSocket connected to {url}");
                };

                this._wsocket.OnMessage += (s, e) =>
                {
                    try
                    {
                        var json = JObject.Parse(e.Data);
                        var evt = json["event"]?.ToString();
                        var data = (JObject)json["data"];

                        OnEventReceived?.Invoke(evt, data);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Ошибка разбора WS: " + ex.Message);
                    }
                };

                this._wsocket.OnError += (s, e) => Logger.Error("WebSocket error: " + e.Message);
                this._wsocket.OnClose += (s, e) =>
                {
                    Logger.Info("WebSocket closed");

                    if (!this._manualClose)
                    {
                        this._reconnectTimer?.Dispose();
                        this._reconnectTimer = new Timer(_ => Connect(AppSession.CurrentUser.UserUid), null, 13000, Timeout.Infinite);
                    }
                };

                this._wsocket.ConnectAsync();
            }
        }

        public void Send(string message)
        {
            if (this._wsocket != null && this._wsocket.IsAlive)
            {
                this._wsocket.Send(message);
            }
        }

        public void Disconnect()
        {
            lock (this._lock)
            {
                this._manualClose = true;

                if (this._wsocket != null && this._wsocket.IsAlive)
                {
                    this._wsocket.Close();
                    Logger.Info("WebSocket manually closed");
                }

                this._reconnectTimer?.Dispose();
                this._reconnectTimer = null;
            }
        }
    }
}
