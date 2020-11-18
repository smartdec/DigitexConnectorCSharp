using System;
using DigitexConnector.EngineAPI;
using DigitexConnector.Orders;

namespace DigitexConnector.Interfaces
{
    public interface IConnection : IDisposable
    {
        event Action<OrderStatusData> OrderStatusEvent;
        event Action<OrderFilledData> OrderFilledEvent;
        event Action<TraderStatusData> TraderStatusEvent;
        event Action<TraderBalanceData> TraderBalanceEvent;
        event Action<FundingData> FundingEvent;
        event Action<OrderCanceledData> OrderCanceledEvent;
        event Action<Guid, ErrorCodes> ErrorEvent;
        event Action<OrderBookFullUpdateData> OrderBookFullUpdateEvent;
        event Action<OrderBookData> OrderBookEvent;
        event Action<ExchangeRateData> ExchangeRateUpdateEvent;
        event Action<MarketStateData> MarketStateEvent;
        event Action<MarketStateUpdateData> MarketStateUpdateEvent;
        event Action<LeverageData> LeverageEvent;
        event Action DataConnected;
        event Action DataReconnected;
        event Action DataDisconnected;
        event Action ControlConnected;
        event Action ControlReconnected;
        event Action ControlDisconnected;
        ITransport GetTransport();
        bool IsDataConnected();
        bool IsControlConnected();
        bool PlaceOrder(OrderBase order);
        bool CancelOrder(Guid requestId, Guid prevRequestId, uint marketId);
        bool CancelAllOrders(Guid requestId, uint marketId);
        bool GetTraderStatus(Guid clientId);
        bool GetOrderBook(Guid guid, uint marketId);
        bool GetMarketState(Guid guid, uint marketId);
        void Connect();
    }
}
