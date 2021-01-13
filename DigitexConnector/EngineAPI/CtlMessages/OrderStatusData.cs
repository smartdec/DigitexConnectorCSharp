using System;
using DigitexWire;
using DigitexConnector.Extentions;

namespace DigitexConnector.EngineAPI
{
    ///<summary>
    ///Order status message
    ///</summary>
    /// <inheritdoc/>
    public class OrderStatusData : CommonFields
    {
        /// <summary>
        /// Order direction.
        /// </summary>
        public OrderSide Direction { get; }

        /// <summary>
        /// Order status.
        /// </summary>
        public OrderStatus Status { get; }

        /// <summary>
        /// Spot price.
        /// </summary>
        public decimal MarkPrice { get; }

        /// <summary>
        /// Price for limit orders. 0 for market orders.
        /// </summary>
        public decimal Price { get; }

        /// <summary>
        /// Order quantity.
        /// </summary>
        public decimal Quantity { get; }

        /// <summary>
        /// Duration of order.
        /// </summary>
        public OrderDuration Duration { get; }

        /// <summary>
        /// Leverage of order.
        /// </summary>
        public uint Leverage { get; }

        /// <summary>
        /// Previous contract id.
        /// </summary>
        public ulong OldContractId { get; }

        /// <summary>
        /// Current order id.
        /// </summary>
        public Guid OrderClientId { get; }

        /// <summary>
        /// Time when order was placed.
        /// </summary>
        public DateTime OrderTimestamp { get; }

        /// <summary>
        /// Type of order.
        /// </summary>
        public OrderType OrderType { get; }

        /// <summary>
        /// Paid price of order.
        /// </summary>
        public decimal PaidPrice { get; }

        /// <summary>
        /// Side of order.
        /// </summary>
        public OrderSide Side { get; }

        /// <summary>
        /// Time of open original order.
        /// </summary>
        public DateTime OpenTime { get; }

        /// <summary>
        /// Id of original order.
        /// </summary>
        public Guid OrigClientId { get; }

        /// <summary>
        /// Quantity of original order.
        /// </summary>
        public decimal OrigQuantity { get; }

        /// <summary>
        /// For spot markets, balance in base currency.
        /// </summary>
        public decimal TraderBalance2 { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="Message"/>
        /// <param name="message">Source message.</param>
        public OrderStatusData(Message message)
        {
            OrderStatusMessage orderStatusMessage = message.OrderStatusMsg.Clone();
            TraderId = message.TraderId;
            ClientId = DWConverter.FromProtoUuid(message.ClientId);
            Symbol = SymbolsContainer.GetSymbol(message.MarketId);
            Direction = orderStatusMessage.Side;
            Status = orderStatusMessage.Status;
            PositionMargin = DWConverter.FromProtoDecimal(orderStatusMessage.PositionMargin);
            OrderMargin = DWConverter.FromProtoDecimal(orderStatusMessage.OrderMargin);
            TraderBalance = DWConverter.FromProtoDecimal(orderStatusMessage.TraderBalance);
            TraderBalance2 = DWConverter.FromProtoDecimal(orderStatusMessage.TraderBalance2);
            MarkPrice = DWConverter.FromProtoDecimal(orderStatusMessage.MarkPrice);
            PaidPrice = DWConverter.FromProtoDecimal(orderStatusMessage.PaidPrice);
            Price = DWConverter.FromProtoDecimal(orderStatusMessage.Price);
            Quantity = DWConverter.FromProtoDecimal(orderStatusMessage.Quantity);
            //this.TakeProfitPrice = message.FromProtoDecimal(orderStatusMessage.TakeProfitPrice);
            Upnl = DWConverter.FromProtoDecimal(orderStatusMessage.Upnl);
            BuyOrderMargin = DWConverter.FromProtoDecimal(orderStatusMessage.BuyOrderMargin);
            Duration = orderStatusMessage.Duration;
            Leverage = orderStatusMessage.Leverage;
            OldContractId = orderStatusMessage.OldContractId;
            OrderClientId = DWConverter.FromProtoUuid(orderStatusMessage.OrderClientId);
            if (orderStatusMessage.OrderTimestamp == 0)
            {
                OrderTimestamp = DWConverter.FromLongDateTime(message.Timestamp);
            }
            else
            {
                OrderTimestamp = DWConverter.FromLongDateTime(orderStatusMessage.OrderTimestamp);
            }
            OrderType = orderStatusMessage.OrderType;
            PaidPrice = DWConverter.FromProtoDecimal(orderStatusMessage.PaidPrice);
            SellOrderMargin = DWConverter.FromProtoDecimal(orderStatusMessage.SellOrderMargin);
            Side = orderStatusMessage.Side;
            //this.TakeProfitType = orderStatusMessage.TakeProfitType;
            Pnl = DWConverter.FromProtoDecimal(orderStatusMessage.Pnl);
            BuyOrderQuantity = DWConverter.FromProtoDecimal(orderStatusMessage.BuyOrderQuantity);
            SellOrderQuantity = DWConverter.FromProtoDecimal(orderStatusMessage.SellOrderQuantity);
            AccumQuantity = DWConverter.FromProtoDecimal(orderStatusMessage.AccumQuantity);
            OpenTime = DWConverter.FromLongDateTime(orderStatusMessage.OpenTime);
            OrigClientId = DWConverter.FromProtoUuid(orderStatusMessage.OrigClientId);
            OrigQuantity = DWConverter.FromProtoDecimal(orderStatusMessage.OrigQuantity);
        }
    }
}
