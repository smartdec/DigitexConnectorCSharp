using DigitexWire;
using DigitexConnector.EngineAPI;
using DigitexConnector.Interfaces;

namespace DigitexConnector.Orders
{
    public class OrderLimit : OrderBase
    {
        public OrderLimit(OrderSide side, decimal quantity, decimal price, Symbol symbol, IAggregator account = null, OrderDuration duration = OrderDuration.Gtc, uint leverage = 1)
            : base(OrderType.Limit,side, quantity, account, symbol, price, duration, leverage)
        { }

        public OrderLimit(OrderStatusData data, Symbol symbol)
            : base(OrderType.Limit, data, symbol)
        { }

        public OrderLimit(OrderMessage orderMessage, Symbol symbol)
            :base(orderMessage, symbol)
        { }
    }
}
