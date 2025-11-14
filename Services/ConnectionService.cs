using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;

namespace Parmigiano.Services
{
    public class ConnectionService
    {
        private static ConnectionService _instance;
        public static ConnectionService Instance => _instance ??= new ConnectionService();

        public WSocketClientService WebSocket { get; private set; }
        public TcpClientService Tcp { get; private set; }

        public event Action<string, JObject?> OnWsEvent;
        public event Action<ResponseStruct.Response> OnTcpEvent;

        public bool IsConnectedWSocket => this.WebSocket != null && this.WebSocket.IsConnected;
        public bool IsConnectedTcp => this.Tcp != null && this.Tcp.IsConnected;

        private ConnectionService()
        {
            // WSocket
            this.WebSocket = new WSocketClientService();
            this.WebSocket.OnEventReceived += (evt, data) => this.OnWsEvent?.Invoke(evt, data);

            // TCP
            this.Tcp = new TcpClientService();
            this.Tcp.OnEventReceived += (packet) => this.OnTcpEvent?.Invoke(packet);
        }

        public void EnsureConnectedWSocket()
        {
            try
            {
                if (!this.IsConnectedWSocket)
                {
                    Logger.Info("ConnectionService: WebSocket не подключен — выполняем подключение...");
                    this.ConnectWSocket();
                }
                else
                {
                    Logger.Info("ConnectionService: WebSocket уже подключен.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"ConnectionService: ошибка при инициализации WebSocket: {ex.Message}");
            }
        }

        public void ConnectWSocket()
        {
            WebSocket.Connect();
        }

        public void ConnectTcp()
        {
            Tcp.Connect();
        }

        public void DisconnectAll()
        {
            WebSocket.Disconnect();
            Tcp.Disconnect();
        }
    }
}
