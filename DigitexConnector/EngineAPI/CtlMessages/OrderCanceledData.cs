using System;
using System.Collections.Generic;
using DigitexWire;
using Google.Protobuf;
using DigitexConnector.Extentions;
using DigitexConnector.Orders;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Order canceled message.
    /// </summary>
    /// <inheritdoc/>
    public class OrderCanceledData : CommonFields
    {
        /// <summary>
        /// Ids of canceled orders.
        /// </summary>
        public List<Guid> OrderIds { get; }

        public List<OrderBase> Orders { get; }

        /// <summary>
        /// Order status.
        /// </summary>
        public OrderStatus Status { get; }

        /// <summary>
        /// Spot price.
        /// </summary>
        public decimal MarkPrice { get; }

        /// <summary>
        /// Id of original order.
        /// </summary>
        public Guid OrigClientId { get; }

        /// <summary>
        /// For spot markets, balance in base currency.
        /// </summary>
        public decimal TraderBalance2 { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="Message"/>
        /// <param name="message">Source message.</param>
        public OrderCanceledData(Message message)
        {
            ClientId = DWConverter.FromProtoUuid(message.ClientId);
 
            OrderCanceledMessage orderCanceledMessage = message.OrderCanceledMsg.Clone();

            OrigClientId = DWConverter.FromProtoUuid(orderCanceledMessage.PrevClientId);
            TraderId = message.TraderId;
            Symbol = SymbolsContainer.GetSymbol(message.MarketId);
            MarkPrice = DWConverter.FromProtoDecimal(orderCanceledMessage.MarkPrice);
            OrderIds = new List<Guid>();
            foreach (ByteString orderId in orderCanceledMessage.OrderIds)
            { 
                OrderIds.Add(DWConverter.FromProtoUuid(orderId));
            }
            OrderMargin = DWConverter.FromProtoDecimal(orderCanceledMessage.OrderMargin);
            PositionMargin = DWConverter.FromProtoDecimal(orderCanceledMessage.PositionMargin);
            Status = orderCanceledMessage.Status;
            TraderBalance = DWConverter.FromProtoDecimal(orderCanceledMessage.TraderBalance);
            TraderBalance2 = DWConverter.FromProtoDecimal(orderCanceledMessage.TraderBalance2);
            Upnl = DWConverter.FromProtoDecimal(orderCanceledMessage.Upnl);
            BuyOrderMargin = DWConverter.FromProtoDecimal(orderCanceledMessage.BuyOrderMargin);
            Pnl = DWConverter.FromProtoDecimal(orderCanceledMessage.Pnl);
            SellOrderMargin = DWConverter.FromProtoDecimal(orderCanceledMessage.SellOrderMargin);
            BuyOrderQuantity = DWConverter.FromProtoDecimal(orderCanceledMessage.BuyOrderQuantity);
            SellOrderQuantity = DWConverter.FromProtoDecimal(orderCanceledMessage.SellOrderQuantity);
            AccumQuantity = DWConverter.FromProtoDecimal(orderCanceledMessage.AccumQuantity);
            Orders = new List<OrderBase>();
            foreach (OrderMessage order in orderCanceledMessage.Orders)
            {
                Orders.Add(order.OrderType == OrderType.Limit ? (OrderBase)new OrderLimit(order, Symbol) :
                                                                new OrderMarket(order, Symbol));
            }
        }
    }
}
