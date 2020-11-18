using DigitexWire;
using DigitexConnector.Orders;

namespace DigitexConnector.Extentions
{
    static class PlaceOrderMessageExtention
    {
        /// <summary>
        /// Create place order message from order base.
        /// </summary>
        /// <param name="placeOrderMessage">Target message. <see cref="PlaceOrderMessage"/></param>
        /// <param name="order">Original order. <see cref="OrderBase"/></param>
        /// <returns></returns>
        static internal PlaceOrderMessage FromOrderBase(this PlaceOrderMessage placeOrderMessage, OrderBase order)
        {
            placeOrderMessage.OrderType = order.Type;
            placeOrderMessage.Side = order.Side;
            placeOrderMessage.Duration = order.Duration;
            placeOrderMessage.Leverage = 1; // Ignored by server.
            placeOrderMessage.Price = DWConverter.ToProtoDecimal(order.Price);
            placeOrderMessage.Quantity = DWConverter.ToProtoDecimal(order.Quantity);
            return placeOrderMessage;
        }
    }
}
