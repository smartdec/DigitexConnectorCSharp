using DigitexWire;
using DigitexConnector.EngineAPI;
using DigitexConnector.Interfaces;

namespace DigitexConnector.Orders
{
    public class OrderMarket : OrderBase
    {
        public OrderMarket(OrderSide position, decimal quantity, Symbol symbol, IAggregator account = null, OrderDuration duration = OrderDuration.Gtc, uint leverage = 1)
            : base(OrderType.Market, position, quantity, account, symbol, duration: duration, leverage: leverage)
        { }

        public OrderMarket(OrderStatusData data, Symbol symbol)
            : base(OrderType.Market, data, symbol)
        { }

        public OrderMarket(OrderMessage orderMessage, Symbol symbol)
            : base(orderMessage, symbol)
        { }
    }
}
