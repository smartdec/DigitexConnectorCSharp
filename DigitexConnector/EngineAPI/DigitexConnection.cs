using System;
using DigitexWire;
using Google.Protobuf;
using DigitexConnector.Orders;
using DigitexConnector.Extentions;
using MessageType = DigitexWire.Message.KontentOneofCase;
using NLog;
using DigitexConnector.Interfaces;
using System.Collections.Generic;
using DigitexConnector.Enums;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// A class that implements sending control messages to the server and receiving response messages via the ctl channel.
    /// </summary>
    public class DigitexConnection : IConnection
    {

        /// <summary>
        /// Event of receiving OrderStatus message.
        /// </summary>
        public event Action<OrderStatusData> OrderStatusEvent;

        /// <summary>
        /// Event of receiving filled orders.
        /// </summary>
        public event Action<OrderFilledData> OrderFilledEvent;

        /// <summary>
        /// Event of receiving TraderStatus message.
        /// </summary>
        public event Action<TraderStatusData> TraderStatusEvent;

        /// <summary>
        /// Event of receiving TraderBalance message.
        /// </summary>
        public event Action<TraderBalanceData> TraderBalanceEvent;

        /// <summary>
        /// Event of receiving Funding message.
        /// </summary>
        public event Action<FundingData> FundingEvent;

        /// <summary>
        /// Event of receiving OrderCanceled message.
        /// </summary>
        public event Action<OrderCanceledData> OrderCanceledEvent;

        /// <summary>
        /// Event of receiving Error message.
        /// </summary>
        public event Action<Guid, ErrorCodes> ErrorEvent;

        /// <summary>
        /// The event that occurs when a stock market update is received.
        /// </summary>
        public event Action<OrderBookFullUpdateData> OrderBookFullUpdateEvent;

        /// <summary>
        /// The event that occurs when a stock market is received.
        /// </summary>
        public event Action<OrderBookData> OrderBookEvent;

        /// <summary>
        /// The event that occurs when a spot price update is received.
        /// </summary>
        public event Action<ExchangeRateData> ExchangeRateUpdateEvent;

        /// <summary>
        /// Thre event that occurs wher a market state is received.
        /// </summary>
        public event Action<MarketStateData> MarketStateEvent;

        /// <summary>
        /// The event that occurs when a market state update is received.
        /// </summary>
        public event Action<MarketStateUpdateData> MarketStateUpdateEvent;

        public event Action<LeverageData> LeverageEvent;

        /// <summary>
        /// Event that happens when connection to data channel is established.
        /// </summary>
        public event Action DataConnected;

        public event Action DataReconnected;

        /// <summary>
        /// Event that happens when connection to data channel is closed.
        /// </summary>
        public event Action DataDisconnected;

        /// <summary>
        /// Event that happens when connection to control channel is established.
        /// </summary>
        public event Action ControlConnected;

        public event Action ControlReconnected;

        /// <summary>
        /// Event that happens when connection to control channel is closed.
        /// </summary>
        public event Action ControlDisconnected;

        private readonly ITransport _transport;

        private List<MessageType> dataMessages = new List<MessageType>() 
        { MessageType.ExchangeRateMsg, MessageType.ExchangeRatesMsg,
            MessageType.MarketStateMsg, MessageType.MarketStateUpdateMsg, MessageType.OrderBookMsg,
            MessageType.OrderBookUpdatedMsg, MessageType.ExchangeRatesMsg 
        };


        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="transport">The object inherited AbstractTransport. <see cref="ITransport"/></param>
        public DigitexConnection(ITransport transport)
        {
            _transport = transport;
            Init();
        }

        public DigitexConnection(string hostName, string token, bool secureConnection)
        {
            _transport = new WebsocketsTransport(hostName, token, secureConnection);
            Init();
        }

        public DigitexConnection(Servers? server, string token)
        {
            _transport = new WebsocketsTransport(server, token);
            Init();
        }

        public DigitexConnection(string token)
        {
            _transport = new WebsocketsTransport(token);
            Init();
        }

        public DigitexConnection()
        {
            _transport = new WebsocketsTransport();
            Init();
        }

        private void Init()
        {
            _transport.MessageReceived += ReceiveMessage;
            _transport.DataConnected += () => DataConnected?.Invoke();
            _transport.DataReconnected += () => DataReconnected?.Invoke();
            _transport.DataDisconnected += () => DataDisconnected?.Invoke();
            _transport.ControlConnected += () => ControlConnected?.Invoke();
            _transport.ControlReconnected += () => ControlReconnected?.Invoke();
            _transport.ControlDisconnected += () => ControlDisconnected?.Invoke();
        }

        private void ReceiveMessage(byte[] ByteMessage)
        {
            Message message;
            try
            {
                message = Message.Parser.ParseFrom(ByteMessage);
                if (!dataMessages.Contains(message.KontentCase))
                { logger.Debug($"Received {message.KontentCase}: {message}"); }
                else
                { logger.Trace($"Received {message.KontentCase}: {message}"); }
            }
            catch (Exception exception)
            {
                logger.Error(exception, ByteMessage.ToString());
                return;
            }
            Guid clientId = DWConverter.FromProtoUuid(message.ClientId);
            if (message.ErrorCode != 0)
            {
                ErrorCodes code = (ErrorCodes)message.ErrorCode;
                ErrorEvent?.Invoke(clientId, code);
            }
            if (message.KontentCase == MessageType.OrderStatusMsg)
            { OrderStatusReceived(message); }
            else if (message.KontentCase == MessageType.OrderFilledMsg)
            { OrderFilledReceived(message); }
            else if (message.KontentCase == MessageType.TraderStatusMsg)
            { TraderStatusReceived(message); }
            else if (message.KontentCase == MessageType.TraderBalanceMsg)
            { TraderBalanceReceived(message); }
            else if (message.KontentCase == MessageType.FundingMsg)
            { FundingReceived(message); }
            else if (message.KontentCase == MessageType.OrderCanceledMsg)
            { OrderCanceledReceived(message); }
            else if (message.KontentCase == MessageType.OrderBookMsg)
            { OrderBookReceived(message); }
            else if (message.KontentCase == MessageType.OrderBookUpdatedMsg)
            { OrderBookUpdatedReceived(message); }
            else if (message.KontentCase == MessageType.ExchangeRateMsg)
            { ExchangeRateReceived(message); }
            else if (message.KontentCase == MessageType.MarketStateMsg)
            { MarketStateReceived(message); }
            else if (message.KontentCase == MessageType.MarketStateUpdateMsg)
            { MarketStateUpdateReceived(message); }
            else if (message.KontentCase == MessageType.LeverageMsg)
            { LeverageReceived(message); }
        }

        private void OrderStatusReceived(Message message)
        {
            OrderStatusData statusData = new OrderStatusData(message);
            OrderStatusEvent?.Invoke(statusData);
        }

        private void OrderFilledReceived(Message message)
        {
            OrderFilledData filledData = new OrderFilledData(message);
            OrderFilledEvent?.Invoke(filledData);
        }

        private void TraderStatusReceived(Message message)
        {
            TraderStatusData traderStatusData = new TraderStatusData(message);
            TraderStatusEvent?.Invoke(traderStatusData);
        }

        private void TraderBalanceReceived(Message message)
        {
            TraderBalanceData data = new TraderBalanceData(message);
            TraderBalanceEvent?.Invoke(data);
        }

        private void FundingReceived(Message message)
        {
            FundingData data = new FundingData(message);
            FundingEvent?.Invoke(data);
        }

        private void OrderCanceledReceived(Message message)
        {
            OrderCanceledData data = new OrderCanceledData(message);
            OrderCanceledEvent?.Invoke(data);
        }

        private void LeverageReceived(Message message)
        {
            LeverageData data = new LeverageData(message);
            LeverageEvent?.Invoke(data);
        }


        private void OrderBookReceived(Message message)
        {
            OrderBookData orderBookFutures = new OrderBookData(message);
            OrderBookEvent?.Invoke(orderBookFutures);
        }

        private void OrderBookUpdatedReceived(Message message)
        {
            OrderBookFullUpdateData data = new OrderBookFullUpdateData(message);
            OrderBookFullUpdateEvent?.Invoke(data);
        }

        private void ExchangeRateReceived(Message message)
        {
            ExchangeRateData exchangeRateFutures = new ExchangeRateData(message);
            ExchangeRateUpdateEvent?.Invoke(exchangeRateFutures);
        }

        private void MarketStateReceived(Message message)
        {
            MarketStateData marketStateData = new MarketStateData(message);
            MarketStateEvent?.Invoke(marketStateData);
        }

        private void MarketStateUpdateReceived(Message message)
        {
            MarketStateUpdateData marketStateUpdateFutures = new MarketStateUpdateData(message);
            MarketStateUpdateEvent?.Invoke(marketStateUpdateFutures);
        }

        /// <summary>
        /// The method of sending a message to place a limit or market order on the exchange.
        /// </summary>
        /// <param name="order">Sending order. <see cref="OrderBase"/></param>
        /// <returns>True if success else false.</returns>
        public bool PlaceOrder(OrderBase order)
        {
            PlaceOrderMessage placeOrderMessage = new PlaceOrderMessage().FromOrderBase(order);
            return SendMessage(placeOrderMessage, (uint)order.TargetSymbol.MarketId, order.ClientId);
        }

        /// <summary>
        /// The method of sending a message to cancel a previously placed limit order on the exchange.
        /// </summary>
        /// <param name="requestId">Id of sending message.</param>
        /// <param name="prevRequestId">Id of the canceled order.</param>
        /// <param name="marketId">Current market id.</param>
        /// <returns>True if success else false.</returns>
        public bool CancelOrder(Guid requestId, Guid prevRequestId, uint marketId)
        {
            CancelOrderMessage cancelOrderMessage = new CancelOrderMessage();
            byte[] bytes = prevRequestId.ToByteArray();
            byte[] newBytes = new byte[]
            {
                    bytes[3],
                    bytes[2],
                    bytes[1],
                    bytes[0],
                    bytes[5],
                    bytes[4],
                    bytes[7],
                    bytes[6],
                    bytes[8],
                    bytes[9],
                    bytes[10],
                    bytes[11],
                    bytes[12],
                    bytes[13],
                    bytes[14],
                    bytes[15],
            };
            Guid uuid = new Guid(newBytes);
            cancelOrderMessage.PrevClientId = ByteString.CopyFrom(uuid.ToByteArray());
            return SendMessage(cancelOrderMessage, marketId, requestId);
        }

        /// <summary>
        /// The method for sending a message to cancel all previously placed limit orders on the exchange.
        /// </summary>
        /// <param name="requestId">Id of sending message.</param>
        /// <param name="marketId">Current market id.</param>
        /// <returns>True if success else false.</returns>
        public bool CancelAllOrders(Guid requestId, uint marketId)
        {
            CancelAllOrdersMessage cancelAllOrdersMessage = new CancelAllOrdersMessage();
            return SendMessage(cancelAllOrdersMessage, marketId, requestId);

        }        
        
        /// <summary>
        /// The method of sending request to get trader status.
        /// </summary>
        /// <param name="clientId">Id of sending message.</param>
        /// <returns>True if success else false.</returns>
        public bool GetTraderStatus(Guid clientId)
        {
            GetTraderStatusMessage getTraderStatusMessage = new GetTraderStatusMessage();
            return SendMessage(getTraderStatusMessage, 1, clientId);
        }

        /// <summary>
        /// The class method for sending request to get a depth of market.
        /// </summary>
        /// <param name="guid">clientId of the sending request.</param>
        /// <param name="marketId">Current market id.</param>
        /// <returns>True if success else false.</returns>
        public bool GetOrderBook(Guid guid, uint marketId)
        {
            OrderBookRequestMessage orderBookRequestMessage = new OrderBookRequestMessage();
            return SendMessage(orderBookRequestMessage, marketId, guid);
        }

        /// <summary>
        /// The class method for sending request to get a state of market.
        /// </summary>
        /// <param name="guid">Id of sending message.</param>
        /// <param name="marketId">Current market id.</param>
        /// <returns>True if success else false.</returns>
        public bool GetMarketState(Guid guid, uint marketId)
        {
            GetMarketStateMessage getMarketStateMessage = new GetMarketStateMessage();
            return SendMessage(getMarketStateMessage, marketId, guid);
        }

        private bool SendMessage(IMessage content, uint marketId, Guid requestId)
        {
            Guid uuid = DWConverter.GuidToUuid(requestId);
            Message message = DWConverter.BuildMessage(content, marketId, uuid, 0);
            byte[] messageByte = message.ToByteArray();
            bool sendResult = _transport.Send(messageByte);
            if (!sendResult)
            {
                logger.Warn($"Error while sending {message.KontentCase}: {message}");
                return false;
            }
            logger.Debug($"Sent {message.KontentCase}: {message}");
            return true;
        }

        /// <summary>
        /// True if connection to data channel is established else false.
        /// </summary>
        public bool IsDataConnected() => _transport.IsDataConnected;

        /// <summary>
        /// True if connection to control channel is established else false.
        /// </summary>
        public bool IsControlConnected() => _transport.IsControlConnected;

        public void Connect() => _transport.Connect();

        public ITransport GetTransport() => _transport;

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            OrderStatusEvent = null;
            OrderFilledEvent = null;
            TraderStatusEvent = null;
            TraderBalanceEvent = null;
            FundingEvent = null;
            OrderCanceledEvent = null;
            ErrorEvent = null;
            OrderBookFullUpdateEvent = null;
            OrderBookEvent = null;
            ExchangeRateUpdateEvent = null;
            MarketStateEvent = null;
            MarketStateUpdateEvent = null;
            DataConnected = null;
            DataReconnected = null;
            DataDisconnected = null;
            ControlConnected = null;
            ControlReconnected = null;
            ControlDisconnected = null;
            MarketStateUpdateEvent = null;
        }
    }
}
