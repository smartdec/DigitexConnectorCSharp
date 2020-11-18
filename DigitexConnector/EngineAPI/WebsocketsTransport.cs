using System;
using System.Threading.Tasks;
using Websocket.Client;
using System.Net.WebSockets;
using NLog;
using DigitexConnector.Interfaces;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Transport class.
    /// </summary>
    public class WebsocketsTransport : ITransport, IDisposable
    {
        private string Token;
        private WebsocketClient DataSocket;
        private WebsocketClient OrderSocket;
        private string HostName;
        private bool SecureConnection;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Event that happens when incoming message is received.
        /// </summary>
        public event Action<byte[]> MessageReceived;

        /// <summary>
        /// Event that happens when transport is connected to data channel.
        /// </summary>
        public event Action DataConnected;

        /// <summary>
        /// Event that happens when transport is reconnected to data channel.
        /// </summary>
        public event Action DataReconnected;

        /// <summary>
        /// Event that happens when transport is disconnected from data channel.
        /// </summary>
        public event Action DataDisconnected;

        /// <summary>
        /// Event that happens when transport is connected to control channel.
        /// </summary>
        public event Action ControlConnected;

        /// <summary>
        /// Event that happens when transport is reconnected to control channel.
        /// </summary>
        public event Action ControlReconnected;

        /// <summary>
        /// Event that happens when transport is desconnected from control channel.
        /// </summary>
        public event Action ControlDisconnected;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="hostName">Exchange address.</param>
        /// <param name="token">Trader's JWT.</param>
        /// <param name="secureConnection">True if connection is secure else false.</param>
        public WebsocketsTransport(string hostName, string token, bool secureConnection = true)
        {
            HostName = hostName;
            Token = token;
            SecureConnection = secureConnection;
        }

        /// <summary>
        /// Send message to exchange.
        /// </summary>
        /// <param name="message">Byte message in proto format. <see cref="DigitexWire.Message"/></param>
        /// <returns></returns>
        public bool Send(byte[] message)
        {
            if (OrderSocket == null || !OrderSocket.IsRunning)
            { return false; }
            OrderSocket.Send(message);
            return true;
        }

        private Func<ClientWebSocket> GetWSFactory(bool needToken = false)
        {
            var factory = new Func<ClientWebSocket>(() =>
            {
                var client = new ClientWebSocket
                {
                    Options =
                        {
                            KeepAliveInterval = TimeSpan.FromSeconds(5),
                        }
                };
                if (needToken)
                { client.Options.SetRequestHeader("Authorization", $"Token {Token}"); }
                return client;
            });
            return factory;
        }

        private async void OpenDataEvents()
        {
            string prefix = SecureConnection ? "wss" : "ws";
            var uri = new Uri($"{prefix}://{HostName}/events/marketdata/");
            DataSocket = new WebsocketClient(uri, GetWSFactory());
            DataSocket.ReconnectTimeout = null;
            DataSocket.IsReconnectionEnabled = true;
            DataSocket.ReconnectionHappened.Subscribe(info => {
                logger.Warn($"Reconnection happened, reason: {info.Type}, stream: data");
                System.Diagnostics.Debug.WriteLine($"Reconnection happened, type: {info.Type}, stream: data");
                DataReconnected?.Invoke();
            });
            DataSocket.DisconnectionHappened.Subscribe(info => {
                logger.Warn($"Disconnection happened, reason: {info.Type}, stream: data");
                DataDisconnected?.Invoke();
            });
            DataSocket.MessageReceived.Subscribe(msg =>
                MessageReceived?.Invoke(msg.Binary));
            await DataSocket.Start();
            DataConnected?.Invoke();
        }

        private async Task OpenTradeEvents()
        {
            string prefix = SecureConnection ? "wss" : "ws";
            var uri = new Uri($"{prefix}://{HostName}/events/order/");
            OrderSocket = new WebsocketClient(uri, GetWSFactory(true));
            OrderSocket.ReconnectTimeout = null;
            OrderSocket.IsReconnectionEnabled = true;
            OrderSocket.ReconnectionHappened.Subscribe(info =>
            {
                logger.Warn($"Reconnection happened, reason: {info.Type}, stream: trade");
                System.Diagnostics.Debug.WriteLine($"Reconnection happened, type: {info.Type}, stream: trade");
                if (info.Type != ReconnectionType.Initial)
                {
                    ControlReconnected?.Invoke();
                }
            });
            OrderSocket.DisconnectionHappened.Subscribe(info =>
            {
                logger.Error($"Disconnected, reason: {info.Type}, exception: {info.Exception}");
                System.Diagnostics.Debug.WriteLine($"Disconnected, type: {info.Type}");
                ControlDisconnected?.Invoke();
                if (info.Type == DisconnectionType.ByServer || info.Type == DisconnectionType.NoMessageReceived)
                {
                    Task.Run(async () => await OpenTradeEvents());
                }
                else
                {
                    logger.Fatal($"Disconnected, reason: {info.Type}, exception: {info.Exception}");
                }
            });
            OrderSocket.MessageReceived.Subscribe(msg => MessageReceived?.Invoke(msg.Binary));
            await OrderSocket.Start();
            ControlConnected?.Invoke();

        }

        /// <summary>
        /// True if transport is connected to data channel. Else false.
        /// </summary>
        public bool IsDataConnected => DataSocket?.IsRunning ?? false;

        /// <summary>
        /// True if transport is connected to control channel. Else false.
        /// </summary>
        public bool IsControlConnected => OrderSocket?.IsRunning ?? false;

        /// <summary>
        /// Connect to exchange.
        /// </summary>
        public void Connect()
        {
            if (OrderSocket == null)
            { OpenTradeEvents(); }
            if (DataSocket == null)
            { OpenDataEvents(); }
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            DataSocket?.Stop(WebSocketCloseStatus.Empty, "");
            DataSocket?.Dispose();
            DataSocket = null;
            OrderSocket?.Stop(WebSocketCloseStatus.Empty, "");
            OrderSocket?.Dispose();
            OrderSocket = null;
            DataConnected = null;
            DataReconnected = null;
            DataDisconnected = null;
            ControlConnected = null;
            ControlReconnected = null;
            ControlDisconnected = null;
            MessageReceived = null;
        }
    }
}
