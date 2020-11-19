using DigitexWire;
using DigitexConnector.EngineAPI;
using DigitexConnector.Interfaces;

namespace DigitexConnector.Orders
{
    public class OrderTrailingStop : OrderMarket
    {
        public OrderTrailingStop(OrderSide position, decimal quantity, decimal adjustedPrice, int lagTicks, Symbol symbol, IAggregator account = null,
            OrderDuration duration = OrderDuration.Gtc, uint leverage = 1)
        : base(position, quantity, symbol, account, duration, leverage)
        {
            LagTicks = lagTicks;
            SetStrikePrice(adjustedPrice);
        }

        public decimal StrikePrice { get; private set; }

        public int LagTicks { internal set; get; }

        internal void SetStrikePrice(decimal adjustedPrice)
        {
            StrikePrice = adjustedPrice + TargetSymbol.PriceStep * LagTicks * (Side == OrderSide.Buy ? 1 : -1);
        }
    }
}
