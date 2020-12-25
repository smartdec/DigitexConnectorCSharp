using System;
using System.Collections.Generic;
using DigitexConnector.EngineAPI;
using System.Linq;
using System.Collections.Concurrent;
using DigitexConnector.Interfaces;

namespace DigitexConnector.Trading
{
    /// <summary>
    /// Order book class.
    /// </summary>
    public class OrderBook : IDisposable
    {
        private readonly IConnection _connection;
        private decimal spotPrice;

        /// <summary>
        /// Event that happens when spot price is updated.
        /// </summary>
        public Action<OrderBook> SpotPriceUpdated;

        /// <summary>
        /// Event that happens when order book is updated.
        /// </summary>
        public Action<OrderBook> OrderBookUpdated;
        
        internal Action<List<Tuple<decimal, decimal>>, List<Tuple<decimal, decimal>>> OrderBookUpdatedList;

        internal Action<List<OHLCVData>> OhlcvReceivedEvent;


        /// <summary>
        /// Event that happens when order book is connected to exchange.
        /// </summary>
        public Action<OrderBook> Connected;

        public Action<OrderBook> Reconnected;

        /// <summary>
        /// Event that happens when order book is disconnected from exchange.
        /// </summary>
        public Action<OrderBook> Disconnected;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connection"><see cref="IConnection"/></param>
        /// <param name="symbol">Target symbol. <see cref="Symbol"/></param>
        public OrderBook(IConnection connection, Symbol symbol) 
        {
            _connection = connection;
            TrackedSymbol = symbol;
            Init();
        }

        private void Init() 
        {
            _connection.OrderBookEvent += OrderBookHandler;
            _connection.OrderBookFullUpdateEvent += OrderBookUpdateHandler;
            _connection.ExchangeRateUpdateEvent += ExchangeRateHandler;
            _connection.MarketStateUpdateEvent += MarketStateUpdateHandler;
            _connection.DataConnected += () => Connected?.Invoke(this);
            _connection.DataReconnected += () => Reconnected?.Invoke(this);
            _connection.DataDisconnected += () => Disconnected?.Invoke(this);
            _connection.GetOrderBook(Guid.NewGuid(), (uint)TrackedSymbol.MarketId);
        }

        private void ExchangeRateHandler(ExchangeRateData data)
        {
            if (data.CurrencyPairId != (uint)TrackedSymbol.CurrencyPairId)
            { return; }
            SpotPrice = data.MarkPrice;
            SpotPriceUpdated?.Invoke(this);
        }

        private void OrderBookHandler(OrderBookData data) 
        {
            if (!TrackedSymbol.Equals(data.Symbol))
            { return; }
            SpotPrice = data.MarkPrice;
            LastTradePrice = data.LastTradePrice;
            LastTradeQuantity = data.LastTradeQuantity;
            Asks = new ConcurrentDictionary<decimal, decimal>(data.Asks);
            Bids = new ConcurrentDictionary<decimal, decimal>(data.Bids);
            LastFullUpdateTimestamp = DateTime.Now;
            OrderBookUpdated?.Invoke(this);
        }

        private void OrderBookUpdateHandler(OrderBookFullUpdateData data)
        {
            if (!TrackedSymbol.Equals(data.Symbol))
            { return; }
            LastTradePrice = data.LastTradePrice;
            LastTradeQuantity = data.LastTradeQuantity;
            SpotPrice = data.MarkPrice;
            foreach (Tuple<decimal, decimal> ask in data.AskUpdates)
            {
                if (ask.Item2 == 0)
                {
                    if (Asks.ContainsKey(ask.Item1))
                    { Asks.TryRemove(ask.Item1, out _); }
                }
                else
                { Asks[ask.Item1] = ask.Item2; }
            }
            foreach (Tuple<decimal, decimal> bid in data.BidUpdates)
            {
                if (bid.Item2 == 0)
                {
                    if (Bids.ContainsKey(bid.Item1))
                    { Bids.TryRemove(bid.Item1, out _); }
                }
                else
                { Bids[bid.Item1] = bid.Item2; }
            }
            LastTrades = data.Trades;
            LastFullUpdateTimestamp = data.LastFullTimestamp;
            OrderBookUpdatedList?.Invoke(data.BidUpdates, data.AskUpdates);
            OrderBookUpdated?.Invoke(this);
        }

        private void MarketStateUpdateHandler(MarketStateUpdateData data)
        {
            if (!TrackedSymbol.Equals(data.Symbol))
            { return; }
            LastTradePrice = data.LastTradePrice;
            LastTradeQuantity = data.LastTradeQuantity;
            PayoutPerContract = data.PayoutPerContract;
            FundingRate = data.FundingRate;
            FundingTime = data.FundingTime;
            SpotPrice = data.SpotPrice;
            OhlcvReceivedEvent?.Invoke(data.Ohlcvs);
        }


        /// <summary>
        /// Best price of asks.
        /// </summary>
        /// <returns>Prise of best ask if asks count is more than zero else null.</returns>
        public decimal? GetBestAskPrice() 
        {
            try
            {
                return Asks.Keys.Min();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Best price of bids.
        /// </summary>
        /// <returns>Price of best bid id bids count is more than zero else null.</returns>
        public decimal? GetBestBidPrice() 
        {
            try
            {
                return Bids.Keys.Max();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Price of last trade on exchange.
        /// </summary>
        public decimal LastTradePrice { private set; get; }

        /// <summary>
        /// Quantity of last trade on exchange.
        /// </summary>
        public decimal LastTradeQuantity { private set; get; }

        /// <summary>
        /// Spot price.
        /// </summary>
        public decimal SpotPrice
        {
            private set 
            { 
                spotPrice = value;
                decimal remainder = value % TrackedSymbol.PriceStep;
                AdjustedSpotPrice = value - remainder + (remainder < (TrackedSymbol.PriceStep / 2) ? 0 : TrackedSymbol.PriceStep);
            }
            get { return spotPrice; }
        }

        /// <summary>
        /// Spot price in subject to tick price.
        /// </summary>
        public decimal AdjustedSpotPrice { private set; get; }

        /// <summary>
        /// Asks collection.
        /// </summary>
        public ConcurrentDictionary<decimal, decimal> Asks { private set; get; } = new ConcurrentDictionary<decimal, decimal>();

        /// <summary>
        /// Bids colletion.
        /// </summary>
        public ConcurrentDictionary<decimal, decimal> Bids { private set; get; } = new ConcurrentDictionary<decimal, decimal>();

        /// <summary>
        /// Last trades collection.
        /// </summary>
        public List<Tuple<decimal, decimal>> LastTrades { get; private set; }

        /// <summary>
        /// Time of last updates of order book.
        /// </summary>
        public DateTime LastFullUpdateTimestamp { internal set; get; }

        /// <summary>
        /// Symbol of order book.
        /// </summary>
        public Symbol TrackedSymbol { get; }

        /// <summary>
        /// True if order book is connected to exhcange ele false.
        /// </summary>
        public bool IsConnected => _connection.IsDataConnected();

        public decimal PayoutPerContract { private set; get; }

        public decimal FundingRate { private set; get; }

        public long FundingTime { private set; get; }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public void Dispose()
        {
            SpotPriceUpdated = null;
            OrderBookUpdated = null;
            OrderBookUpdatedList = null;
            Connected = null;
            Disconnected = null;
            Reconnected = null;
            OhlcvReceivedEvent = null;
        }
    }
}
