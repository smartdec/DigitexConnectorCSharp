using System.Collections.Generic;
using DigitexWire;
using DigitexConnector.Extentions;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Data of order book message.
    /// </summary>
    public class OrderBookData : OrderBookCommonFields
    {
        /// <summary>
        /// Dictionary of asks, where key is price and value is quantitiy.
        /// </summary>
        public Dictionary<decimal, decimal> Asks { get; }

        /// <summary>
        /// Dictionary of bids, where key is price and value is quantitiy.
        /// </summary>
        public Dictionary<decimal, decimal> Bids { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="Message"/>
        /// <param name="message">Source message.</param>
        public OrderBookData(Message message)
        {
            Symbol = SymbolsContainer.GetSymbol(message.MarketId);
            OrderBookMessage orderBookMessage = message.OrderBookMsg.Clone();
            MarketId = message.MarketId;
            MarkPrice = DWConverter.FromProtoDecimal(orderBookMessage.MarkPrice);
            LastTradePrice = DWConverter.FromProtoDecimal(orderBookMessage.LastTradePrice);
            Asks = new Dictionary<decimal, decimal>();
            Bids = new Dictionary<decimal, decimal>();
            foreach (OrderBookEntryMessage ask in orderBookMessage.Asks)
            {
                decimal price = DWConverter.FromProtoDecimal(ask.Price);
                decimal quantity = DWConverter.FromProtoDecimal(ask.Quantity);
                Asks.Add(price, quantity);
            }
            foreach (OrderBookEntryMessage bid in orderBookMessage.Bids)
            {
                decimal price = DWConverter.FromProtoDecimal(bid.Price);
                decimal quantity = DWConverter.FromProtoDecimal(bid.Quantity);
                Bids.Add(price, quantity);
            }
            LastTradeQuantity = DWConverter.FromProtoDecimal(orderBookMessage.LastTradeQuantity);
        }
    }
}
