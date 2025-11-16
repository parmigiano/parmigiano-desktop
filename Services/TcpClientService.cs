using ClientRequestStruct;
using Google.Protobuf;
using Parmigiano.Core;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Parmigiano.Services
{
    public class TcpClientService
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;

        public event Action<ResponseStruct.Response> OnEventReceived;
        public bool IsConnected => _tcpClient != null && _tcpClient.Connected;

        public async void Connect()
        {
            if (this.IsConnected)
            {
                Logger.Tcp("TcpClientService: уже подключено, пропускаем повторное подключение.");
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
            while (!token.IsCancellationRequested)
            {
                try
                {
                    byte[] lengthBytes = await this.ReadExactAsync(4, token);
                    if (lengthBytes == null)
                    {
                        break;
                    }

                    if (!BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(lengthBytes);
                    }

                    int payloadLength = BitConverter.ToInt32(lengthBytes, 0);

                    if (payloadLength <= 0)
                    {
                        Logger.Tcp($"Invalid payload length: {payloadLength}");
                        break;
                    }

                    byte[] payload = await ReadExactAsync(payloadLength, token);
                    if (payload == null)
                    {
                        break;
                    }

                    Logger.Tcp($"DATA[{payload.Length}] {BitConverter.ToString(payload).Replace("-", "")}");

                    var request = ResponseStruct.Response.Parser.ParseFrom(payload);
                    this.OnEventReceived?.Invoke(request);
                }
                catch (Exception ex)
                {
                    Logger.Error("[ERROR] TCP Error: " + ex.Message);
                    break;
                }
            }

            this.Disconnect();
        }

        public async Task SendProtoAsync(IMessage message)
        {
            if (!this.IsConnected)
            {
                Logger.Tcp("TCP: попытка отправки при отсутствии соединения");
                return;
            }

            try
            {
                using var ms = new MemoryStream();
                message.WriteTo(ms);
                byte[] body = ms.ToArray();

                int length = body.Length;
                byte[] lengthBytes = new byte[4];

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBytes);
                }

                byte[] packet = new byte[lengthBytes.Length + body.Length];

                Array.Copy(lengthBytes, 0, packet, 0, lengthBytes.Length);
                Array.Copy(body, 0, packet, lengthBytes.Length, body.Length);

                Logger.Tcp($"bytes {packet.Length}: {BitConverter.ToString(packet).Replace("-", "")}");

                await this._stream.WriteAsync(packet, 0, packet.Length);
            }
            catch (Exception ex)
            {
                Logger.Tcp("Failed send to TCP: " + ex.Message);
            }
        }

        private async Task<byte[]> ReadExactAsync(int length, CancellationToken token)
        {
            byte[] buffer = new byte[length];
            int offset = 0;

            while (offset < length)
            {
                int read = await this._stream.ReadAsync(buffer, offset, length - offset, token);
                if (read == 0)
                {
                    return null;
                }

                offset += read;
            }

            return buffer;
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
