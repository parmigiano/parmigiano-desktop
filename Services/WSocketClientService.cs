using Newtonsoft.Json.Linq;
using Parmigiano.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;
using System.Text;
using System.Threading.Tasks;

namespace Parmigiano.Services
{
    public class WSocketClientService
    {
        private WebSocket _wsocket;
        public event Action<string, JObject> OnEventReceived;

        public void Connect(string token = null)
        {
            string url = $"{Config.Current.WSOCKET_SERVER_ADDR}/wsocket";

            this._wsocket = new WebSocket(url);

            this._wsocket.OnOpen += (s, e) =>
            {
                Logger.Info("WebSocket connected");
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
            this._wsocket.OnClose += (s, e) => Logger.Info("WebSocket closed");

            this._wsocket.ConnectAsync();
        }

        public void Disconnect()
        {
            if (this._wsocket != null && this._wsocket.IsAlive)
            {
                this._wsocket.Close();
            }
        }
    }
}
