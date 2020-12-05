using System;
using System.Collections.Generic;
using DigitexWire;
using DigitexConnector.Orders;
using DigitexConnector.EngineAPI;
using DigitexConnector.Interfaces;
using DigitexConnector.Enums;

namespace DigitexConnector.Trading
{
    /// <summary>
    /// Main class for trading.
    /// </summary>
    public class TradingAdapter : IDisposable
    {
        /// <summary>
        /// Entry-point to exchange.
        /// </summary>
        /// <see cref="IAggregator"/>
        public readonly IAggregator Account;

        /// <summary>
        /// Event that happens when one of tracked spot price updated.
        /// </summary>
        /// <see cref="OrderBook"/>
        public Action<OrderBook> SpotPriceUpdated;

        /// <summary>
        /// Event that happens when one of tracked order books updated.
        /// </summary>
        /// <see cref="OrderBook"/>
        public Action<OrderBook> OrderBookUpdated;

        /// <summary>
        /// Event that happens when one of tracked order books updated.
        /// </summary>
        public Action<List<Tuple<decimal, decimal>>, List<Tuple<decimal, decimal>>> OrderBookUpdatedList;

        /// <summary>
        /// Event that happens when trader info updated.
        /// </summary>
        public Action<TraderInfo> TraderInfoUpdated;

        /// <summary>
        /// Event that happens when trader's new trades received.
        /// </summary>
        /// <see cref="Trade"/>
        public Action<List<Trade>, List<Trade>> NewTradesReceived;

        /// <summary>
        /// Event that happens when new OHLCV received.
        /// </summary>
        public Action<List<OHLCVData>> OhlcvReceived;

        /// <summary>
        /// Event that happens when Account is connected to exchange.
        /// </summary>
        public Action AccountConnected;

        /// <summary>
        /// Event that happens when Account is disconnected from exhcange.
        /// </summary>
        /// <see cref="IAggregator"/>
        public Action AccountDisconnected;

        /// <summary>
        /// Event that happens when Account is disconnected from exhcange.
        /// </summary>
        /// <see cref="IAggregator"/>
        public Action AccountReconnected;

        /// <summary>
        /// Event that happens when one of tracked order books is connected to exchange.
        /// </summary>
        /// <see cref="OrderBook"/>
        public Action<OrderBook> OrderBookConnected;

        /// <summary>
        /// Event that happens when one of tracked order books is disconnected from exchange.
        /// </summary>
        /// <see cref="OrderBook"/>
        public Action<OrderBook> OrderBookDisconnected;

        /// <summary>
        /// Event that happens when one of tracked order books is reconnected to exchange.
        /// </summary>
        /// <see cref="OrderBook"/>
        public Action<OrderBook> OrderBookReconnected;

        public TradingAdapter(IAggregator account)
        {
            Account = account;
            Init();
        }

        /// <summary>
        /// Use this constructor for set hostName and token directly.
        /// </summary>
        /// <param name="hostName">Address of exchange without prefix.</param>
        /// <param name="token">API-token.</param>
        /// <param name="secureConnection">Use (true) ssh or not (false).</param>
        public TradingAdapter(string hostName, string token, bool secureConnection)
        {
            Account = new Aggregator(hostName, token, secureConnection);
            Init();
        }

        /// <summary>
        /// Use this constructor for set one of two servers and set token directly.
        /// </summary>
        /// <param name="server"><see cref="Servers"/></param>
        /// <param name="token">API-token.</param>
        public TradingAdapter(Servers? server, string token)
        {
            Account = new Aggregator(server, token);
            Init();
        }

        /// <summary>
        /// Use this constructor for set token directly and if <see cref="Configuration.Server"/> is set.
        /// </summary>
        /// <param name="token">API-token.</param>
        public TradingAdapter(string token)
        {
            Account = new Aggregator(token);
            Init();
        }

        /// <summary>
        /// Use this constructor if <see cref="Configuration.Server"/> and <see cref="Configuration.Token"/> are set.
        /// </summary>
        public TradingAdapter()
        {
            Account = new Aggregator();
            Init();
        }

        private void Init()
        {
            Account.Connected += OnAccountConnectedHandler;
            Account.Disconnected += OnAccountDisconnectedHandler;
            Account.NewTrades += OnNewTradesReceivedHandler;
            Account.Reconnected += OnAccountReconnectedHandler;
            Account.TraderInfoChanged += OnTraderInfoHandler;
        }

        /// <summary>
        /// Track symbol.
        /// </summary>
        /// <param name="symbol">Symbol that needs to track.</param>
        /// <returns>Tracked order book if success else null. <see cref="OrderBook"/></returns>
        public OrderBook TrackSymbol(Symbol symbol) => 
            Account.TrackSymbol(symbol, OnSpotPriceUpdated, OnOrderBookUpdated, OnOrderBookUpdatedList, 
                OnOhlcvReceivedHandler, OnOrderBookConnectedHandler,
                OnOrderBookDisconnectedHandler, OnOrderBookReconnectedHandler);

        private void OnSpotPriceUpdated(OrderBook orderBook) => SpotPriceUpdated?.Invoke(orderBook);

        private void OnOrderBookUpdated(OrderBook orderBook) => OrderBookUpdated?.Invoke(orderBook);

        private void OnOrderBookUpdatedList(List<Tuple<decimal, decimal>> bidUpdates, 
            List<Tuple<decimal, decimal>> askUpdates) => 
            OrderBookUpdatedList?.Invoke(bidUpdates, askUpdates);

        private void OnOhlcvReceivedHandler(List<OHLCVData> ohlcvs) => OhlcvReceived?.Invoke(ohlcvs);

        /// <summary>
        /// Send to exchange command to place limit order.
        /// </summary>
        /// <param name="symbol">Symbol of order.<see cref="Symbol"/></param>
        /// <param name="side">Buy or Sell. <see cref="OrderSide"/></param>
        /// <param name="quantity">Quantity of order.</param>
        /// <param name="price">Price of order.</param>
        /// <param name="orderStatusHandler">Method that will called when order status received.</param>
        /// <param name="errorHandler">Method that will called when order error received.</param>
        /// <returns>OrderLimit if success else null. <see cref="OrderLimit"/></returns>
        public OrderLimit PlaceOrderLimit(Symbol symbol, OrderSide side, decimal quantity, decimal price, 
            Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null) => 
            Account?.PlaceOrderLimit(symbol, side, quantity, price, orderStatusHandler, errorHandler);

        /// <summary>
        /// Send to exchange command to place market order.
        /// </summary>
        /// <param name="symbol">Symbol of order.<see cref="Symbol"/></param>
        /// <param name="side">Buy or Sell. <see cref="OrderSide"/></param>
        /// <param name="quantity">Quantity of order.</param>
        /// <param name="orderStatusHandler">Method that will called when order status received.</param>
        /// <param name="errorHandler">Method that will called when order error received.</param>
        /// <returns>OrderMarket if success else null. <see cref="OrderLimit"/></returns>
        public OrderMarket PlaceOrderMarket(Symbol symbol, OrderSide side, decimal quantity,
            Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null) =>
            Account.PlaceOrderMarket(symbol, side, quantity, orderStatusHandler, errorHandler);

        /// <summary>
        /// Send to exchange command to place market order if a certain conditions.
        /// </summary>
        /// <param name="symbol">Symbol of order. <see cref="Symbol"/></param>
        /// <param name="side">Buy or Sell. <see cref="OrderSide"/></param>
        /// <param name="quantity">Quantity of order.</param>
        /// <param name="lagTicks">The number of ticks that the tracked price lags behind the spot.</param>
        /// <param name="orderStatusHandler">Method that will called when order status received.</param>
        /// <param name="errorHandler">Method that will called when order error received.</param>
        /// <returns>OrderTrailingStop if success else null. <see cref="OrderTrailingStop"/></returns>
        public OrderTrailingStop PlaceOrderTrailingStop(Symbol symbol, OrderSide side, decimal quantity, int lagTicks, 
            Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null) => 
            Account.PlaceOrderTrailingStop(symbol, side, quantity, lagTicks, orderStatusHandler, errorHandler);

        /// <summary>
        /// Send to exchange command to cancel limit order.
        /// </summary>
        /// <param name="order">Order that needs to cancel. <see cref="OrderBase"/></param>
        /// <returns>True if success else false.</returns>
        public bool CancelOrder(OrderBase order) => Account.CancelOrder(order);

        /// <summary>
        /// Send to exchange command to cancel all of trader's orders.
        /// </summary>
        /// <param name="symbol">Current symbol. <see cref="Symbol"/></param>
        /// <returns>True if success else false.</returns>
        public bool CancelAllOrders(Symbol symbol) => Account.CancelAllOrders(symbol);

        private void OnTraderInfoHandler(TraderInfo traderInfo) => TraderInfoUpdated?.Invoke(traderInfo);

        private void OnNewTradesReceivedHandler(List<Trade> updatedPositions, List<Trade> trades) =>
            NewTradesReceived?.Invoke(updatedPositions, trades);

        private void OnAccountConnectedHandler() => AccountConnected?.Invoke();

        private void OnAccountDisconnectedHandler() => AccountDisconnected?.Invoke();

        private void OnAccountReconnectedHandler() => AccountReconnected?.Invoke();

        private void OnOrderBookConnectedHandler(OrderBook orderBook) => OrderBookConnected?.Invoke(orderBook);

        private void OnOrderBookReconnectedHandler(OrderBook orderBook) => OrderBookReconnected?.Invoke(orderBook);

        private void OnOrderBookDisconnectedHandler(OrderBook orderBook) => OrderBookDisconnected?.Invoke(orderBook);

        public void Connect() => Account.Connect();

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            SpotPriceUpdated = null;
            OrderBookUpdated = null;
            OrderBookUpdatedList = null;
            TraderInfoUpdated = null;
            NewTradesReceived = null;
            AccountConnected = null;
            AccountReconnected = null;
            AccountDisconnected = null;
            OrderBookConnected = null;
            OrderBookReconnected = null; ;
            OrderBookDisconnected = null;
        }
    }
}
