using System;
using System.Collections.Generic;
using DigitexConnector.Orders;
using DigitexWire;
using DigitexConnector.EngineAPI;
using System.Threading;
using NLog;
using DigitexConnector.Interfaces;
using System.Linq;

namespace DigitexConnector.Trading
{
    public class Aggregator : IAggregator
    {
        /// <summary>
        /// Collection of actives orders.
        /// </summary>
        public NotificationDictionary<Guid, OrderBase> Orders { get; } = new NotificationDictionary<Guid, OrderBase>();

        /// <summary>
        /// Collection of actives trailing stop orders.
        /// </summary>
        public List<OrderTrailingStop> TrailingOrders { get; } = new List<OrderTrailingStop>();

        /// <summary>
        /// Trader's indicators from exchange.
        /// </summary>
        private readonly List<TraderInfo> TraderStatistic = new List<TraderInfo>();

        private ReaderWriterLockSlim ordersLock = new ReaderWriterLockSlim();
        private ReaderWriterLockSlim trailingsLock = new ReaderWriterLockSlim();

        private readonly IConnection _connection;
        private readonly Sender _sender;
        private readonly Receiver _receiver;
        private readonly Dictionary<Symbol, OrderBook> _orderBooks = new Dictionary<Symbol, OrderBook>();

        /// <summary>
        /// Event that happens when one of trader's indicators is changed.
        /// </summary>
        public event Action<TraderInfo> TraderInfoChanged;

        /// <summary>
        /// Event that happens when new trades is received.
        /// </summary>
        /// <see cref="Trade"/>
        public event Action<List<Trade>, List<Trade>> NewTrades;

        /// <summary>
        /// Event that happens when Aggregator is connected to exchange.
        /// </summary>
        public event Action Connected;

        /// <summary>
        /// Event that happens when Aggregator is disconnnected from exhcange.
        /// </summary>
        public event Action Disconnected;

        /// <summary>
        /// Event that happens when Aggregator is recconnected to the exhcange.
        /// </summary>
        public event Action Reconnected;

        public event Action<ErrorCodes> NewErrorReceived;

        public event Action<OrderCanceledData> OrderCancelError;

        private Logger _logger = LogManager.GetCurrentClassLogger();

        public Aggregator(IConnection connection)
        {
            _connection = connection;
            _sender = new Sender(Orders, TrailingOrders, _connection, ordersLock, trailingsLock);
            _receiver = new Receiver(Orders, ordersLock);
            Init();
        }

        public Aggregator(string hostName, string token, bool secureConnection)
        {
            _connection = new DigitexConnection(hostName, token, secureConnection);
            _sender = new Sender(Orders, TrailingOrders, _connection, ordersLock, trailingsLock);
            _receiver = new Receiver(Orders, ordersLock);
            Init();
        }
        
        private void Init()
        {
            foreach (string symbolName in SymbolsContainer.GetSymbolNames())
            {
                TraderStatistic.Add(new TraderInfo(SymbolsContainer.GetSymbol(symbolName)));
            }
            _connection.OrderStatusEvent += OrderStatusHandler;
            _connection.OrderFilledEvent += OrderFilledHandler;
            _connection.TraderBalanceEvent += TraderBalanceHandler;
            _connection.TraderStatusEvent += TraderStatusHandler;
            _connection.FundingEvent += FundingHandler;
            _connection.OrderCanceledEvent += OrderCanceledHandler;
            _connection.LeverageEvent += LeverageHandler;
            _connection.ErrorEvent += ErrorHandler;
            _connection.ControlConnected += () =>
            {
                UpdateTraderStatus();
                Connected?.Invoke();
            };
            _connection.ControlReconnected += () => {
                UpdateTraderStatus();
                Reconnected?.Invoke();
            };
            _connection.ControlDisconnected += () => Disconnected?.Invoke();
        }

        private void OrderStatusHandler(OrderStatusData data) 
        {
            TraderInfo traderInfo = TraderStatistic.Find(x => x.Symbol.Equals(data.Symbol));
            if (traderInfo != null)
            { 
                traderInfo.Update(data);
                TraderInfoChanged?.Invoke(traderInfo);
            }
            _receiver.OrderStatusHandle(data);
        }

        private void OrderFilledHandler(OrderFilledData data)
        {
            TraderInfo traderInfo = TraderStatistic.Find(x => x.Symbol.Equals(data.Symbol));
            if (traderInfo != null)
            {
                traderInfo.Update(data);
                TraderInfoChanged?.Invoke(traderInfo);
            }

            // Code below is usefull?
            List<Trade> updatedPositions = new List<Trade>();
            List<Trade> allTrades = new List<Trade>();
            foreach (Trade trade in data.Trades)
            { 
                trade.Symbol = data.Symbol; 
                updatedPositions.Add(trade); 
            }

            foreach (Trade trade in data.RawTrades)
            {
                trade.Symbol = data.Symbol;
                allTrades.Add(trade);
            }
            NewTrades?.Invoke(updatedPositions, allTrades);

            _receiver.OrderFilledHandle(data);
        }

        private void TraderBalanceHandler(TraderBalanceData data) 
        {
            TraderInfo traderInfo = TraderStatistic.Find(x => x.Symbol.Equals(data.Symbol));
            if (traderInfo != null)
            {
                traderInfo.Update(data);
                TraderInfoChanged?.Invoke(traderInfo);
            }
        }

        private void TraderStatusHandler(TraderStatusData data)
        {
            TraderInfo traderInfo = TraderStatistic.Find(x => x.Symbol.Equals(data.Symbol));
            if (traderInfo != null)
            {
                traderInfo.Update(data);
                TraderInfoChanged?.Invoke(traderInfo);
            }
            _receiver.TraderStatusHandle(data);

            // Code below is usefull?
            List<Trade> allTrades = new List<Trade>();
            foreach (Trade trade in data.Trades)
            { trade.Symbol = data.Symbol; allTrades.Add(trade); }
            NewTrades?.Invoke(new List<Trade>(), allTrades);
        }

        private void FundingHandler(FundingData data)
        {
            TraderInfo traderInfo = TraderStatistic.Find(x => x.Symbol.Equals(data.Symbol));
            if (traderInfo != null)
            {
                traderInfo.Update(data);
                TraderInfoChanged?.Invoke(traderInfo);
            }

            // Code below is usefull?
            List<Trade> allTrades = new List<Trade>();
            foreach (Trade trade in data.Trades)
            { trade.Symbol = data.Symbol; allTrades.Add(trade); }
            NewTrades?.Invoke(new List<Trade>(), allTrades);
        }

        private void OrderCanceledHandler(OrderCanceledData data) 
        {
            TraderInfo traderInfo = TraderStatistic.Find(x => x.Symbol.Equals(data.Symbol));
            if (traderInfo != null)
            {
                traderInfo.Update(data);
                TraderInfoChanged?.Invoke(traderInfo);
            }
                _receiver.OrderCanceledHandle(data);

            if (data.Status == OrderStatus.Rejected)
            {
                OrderCancelError?.Invoke(data);
            }
        }

        private void LeverageHandler(LeverageData data)
        {
            TraderInfo traderInfo = TraderStatistic.Find(x => x.Symbol.Equals(data.Symbol));
            if (traderInfo != null)
            {
                traderInfo.Update(data);
                _receiver.UpdateOrders(data.Orders);
            }
        }

        private void ErrorHandler(Guid guid, ErrorCodes errorCode)
        {
            NewErrorReceived?.Invoke(errorCode);
            _receiver.ErrorHandle(guid, errorCode);
        }

        private void CheckTrailingstopOrders(OrderBook orderBook) => _sender.CheckTrailingStopOrders(orderBook);
        
        public OrderLimit PlaceOrderLimit(Symbol symbol, OrderSide side, decimal quantity, decimal price, 
            Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null)
        {
            OrderLimit order = new OrderLimit(side, quantity, price, symbol, this);
            return _sender.PlaceOrderLimit(order, orderStatusHandler, errorHandler);
        }
        
        public OrderMarket PlaceOrderMarket(Symbol symbol, OrderSide side, decimal quantity, 
            Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null)
        {
            OrderMarket order = new OrderMarket(side, quantity, symbol, this);
            return _sender.PlaceOrderMarket(order, orderStatusHandler, errorHandler);
        }
        
        public OrderTrailingStop PlaceOrderTrailingStop(Symbol symbol, OrderSide side, decimal quantity, int lagTicks, 
            Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null)
        {
            OrderTrailingStop order = new OrderTrailingStop(side, quantity, _orderBooks[symbol].AdjustedSpotPrice, lagTicks, symbol, this);
            return _sender.PlaceOrderTrailingStop(order, orderStatusHandler, errorHandler);
        }

        public bool CancelOrder(OrderBase order) => _sender.CancelOrder(order);

        public bool CancelAllOrders(Symbol symbol) => _sender.CancelAllOrders(symbol);

        public bool UpdateTraderStatus() => _sender.UpdateTraderStatus();

        /// <summary>
        /// Create OrderBook that track for selected symbol
        /// </summary>
        /// <param name="symbol">Tracked symbol. <see cref="Symbol"/></param>
        /// <param name="spotPriceUpdatedHandler">Call if spot price is updated.</param>
        /// <param name="orderBookUpdatedHandler">Call if orderbook is updated.</param>
        /// <param name="orderBookUpdatedInListHandler">Call if orderbook is updated (with tuple of updates).</param>
        /// <param name="ohlcvReceivedHandler">Call if OHLCV is received.</param>
        /// <param name="orderBookConnectedHandler">Call if orderbook is connected to exchange.</param>
        /// <param name="orderBookDisconnectedHandler">Call if orderbook is disconnected from exchange.</param>
        /// <param name="orderBookReconnecctedHandler">Call if orderbook is reconnected to exchange.</param>
        /// <returns>Instance of OrderBook which track for selected symbol. <see cref="OrderBook"/></returns>
        public OrderBook TrackSymbol(
            Symbol symbol, Action<OrderBook> spotPriceUpdatedHandler = null, Action<OrderBook> orderBookUpdatedHandler = null, 
            Action<List<Tuple<decimal, decimal>>, List<Tuple<decimal, decimal>>> orderBookUpdatedInListHandler = null,
            Action<List<OHLCVData>> ohlcvReceivedHandler = null, Action<OrderBook> orderBookConnectedHandler = null, 
            Action<OrderBook> orderBookDisconnectedHandler = null, Action<OrderBook> orderBookReconnecctedHandler = null)
        {
            if (symbol == null)
            { return null; }    
            OrderBook orderBook = null;
            if (_orderBooks.ContainsKey(symbol))
            { orderBook = _orderBooks[symbol]; }
            else if (SymbolsContainer.Contains(symbol))
            {
                orderBook = new OrderBook(_connection, symbol);
                orderBook.SpotPriceUpdated += CheckTrailingstopOrders;
                orderBook.SpotPriceUpdated += spotPriceUpdatedHandler;
                orderBook.OrderBookUpdated += orderBookUpdatedHandler;
                orderBook.OrderBookUpdatedList += orderBookUpdatedInListHandler;
                orderBook.OhlcvReceivedEvent += ohlcvReceivedHandler;
                orderBook.Connected += orderBookConnectedHandler;
                orderBook.Disconnected += orderBookDisconnectedHandler;
                orderBook.Reconnected += orderBookReconnecctedHandler;
                _orderBooks.Add(symbol, orderBook);
            }
            return orderBook;
        }

        /// <summary>
        /// True if connected else false.
        /// </summary>
        public bool IsConnected() => _connection.IsControlConnected();

        public TraderInfo GetTraderInfo(Symbol symbol) => TraderStatistic.Find(x => x.Symbol.Equals(symbol));

        public List<OrderBase> GetOrders() => Orders.Values.ToList();

        public List<OrderBase> GetOrders(Symbol symbol) => Orders.Values.Where(x => x.TargetSymbol.Equals(symbol)).ToList();

        public void Connect() => _connection.Connect();

        public IConnection GetConnection() => _connection;

        public void Dispose()
        {
            foreach (Symbol symbol in _orderBooks.Keys)
            { _orderBooks[symbol].Dispose(); }
            _orderBooks.Clear();
            foreach (Guid guid in Orders.Keys)
            { Orders[guid].Dispose(); }
            Orders.Clear();
            foreach (OrderBase order in TrailingOrders)
            { order.Dispose(); }
            TrailingOrders.Clear();
            TraderInfoChanged = null;
            NewTrades = null;
            Connected = null;
            Disconnected = null;
            Reconnected = null;
        }

        ~Aggregator()
        {
            ordersLock?.Dispose();
            trailingsLock?.Dispose();
        }
    }
}
