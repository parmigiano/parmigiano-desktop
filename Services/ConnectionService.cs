using Newtonsoft.Json.Linq;
using System;

namespace Parmigiano.Services
{
    public class ConnectionService
    {
        private static ConnectionService _instance;
        public static ConnectionService Instance => _instance ??= new ConnectionService();

        public WSocketClientService WebSocket { get; private set; }
        // public TcpClientService Tcp { get; private set; }

        public event Action<string, JObject> OnWsEvent;

        private ConnectionService()
        {
            this.WebSocket = new WSocketClientService();
            this.WebSocket.OnEventReceived += (evt, data) => OnWsEvent?.Invoke(evt, data);

            // TCP
        }

        public void ConnectWSocket()
        {
            WebSocket.Connect();
        }

        public void ConnectTcp()
        {
            // Tcp.Connect();
        }

        public void DisconnectAll()
        {
            WebSocket.Disconnect();
            // Tcp.Disconnect();
        }
    }
}
