using System;
using System.Collections.Generic;
using DigitexConnector.Orders;
using DigitexConnector.Trading;
using DigitexConnector.EngineAPI;
using System.Threading.Tasks;
using DigitexWire;

namespace DigitexConnector.Interfaces
{
    public interface IAggregator : IDisposable
    {
        event Action<TraderInfo> TraderInfoChanged;
        event Action<List<Trade>, List<Trade>> NewTrades;
        event Action Connected;
        event Action Disconnected;
        event Action Reconnected;
        event Action<ErrorCodes> NewErrorReceived;
        event Action<OrderCanceledData> OrderCancelError;
        IConnection GetConnection();
        OrderLimit PlaceOrderLimit(Symbol symbol, OrderSide side, decimal quantity, decimal price,
            Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null);
        OrderMarket PlaceOrderMarket(Symbol symbol, OrderSide side, decimal quantity,
            Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null);
        OrderTrailingStop PlaceOrderTrailingStop(Symbol symbol, OrderSide side, decimal quantity, int lagTicks,
            Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null);
        bool CancelOrder(OrderBase order);
        bool CancelAllOrders(Symbol symbol);
        bool UpdateTraderStatus();
        OrderBook TrackSymbol(
            Symbol symbol, Action<OrderBook> spotPriceUpdatedHandler = null, Action<OrderBook> orderBookUpdatedHandler = null,
            Action<List<Tuple<decimal, decimal>>, List<Tuple<decimal, decimal>>> orderBookUpdatedInListHandler = null,
            Action<List<OHLCVData>> ohlcvReceivedHandler = null, Action<OrderBook> orderBookConnectedHandler = null,
            Action<OrderBook> orderBookDisconnectedHandler = null, Action< OrderBook> orderBookReconnectedHandler = null);
        bool IsConnected();
        TraderInfo GetTraderInfo(Symbol symbol);
        List<OrderBase> GetOrders();
        List<OrderBase> GetOrders(Symbol symbol);
        void Connect();
    }
}
