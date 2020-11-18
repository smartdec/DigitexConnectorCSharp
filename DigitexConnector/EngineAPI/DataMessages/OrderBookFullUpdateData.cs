using System;
using System.Collections.Generic;
using DigitexWire;
using DigitexConnector.Extentions;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Data of order book full updated message.
    /// </summary>
    public class OrderBookFullUpdateData : OrderBookCommonFields
    {
        /// <summary>
        /// Last timestamp when OrderBookFullUpdateData was received.
        /// </summary>
        public DateTime LastFullTimestamp { get; }

        /// <summary>
        /// Update serial.
        /// </summary>
        public long UpdateSerial { get; }

        /// <summary>
        /// Chanegs in asks from previous state.
        /// </summary>
        public List<Tuple<decimal, decimal>> AskUpdates { get; }

        /// <summary>
        /// Chanegs in bids from previous state.
        /// </summary>
        public List<Tuple<decimal, decimal>> BidUpdates { get; }

        /// <summary>
        /// List of all trades at the timestamp.
        /// </summary>
        public List<Tuple<decimal, decimal>> Trades { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="Message"/>
        /// <param name="message">Source message.</param>
        public OrderBookFullUpdateData(Message message)
        {
            Symbol = SymbolsContainer.GetSymbol(message.MarketId);
            OrderBookUpdatedMessage orderBookUpdatedMessage = message.OrderBookUpdatedMsg.Clone();
            MarketId = message.MarketId;
            AskUpdates = new List<Tuple<decimal, decimal>>();
            BidUpdates = new List<Tuple<decimal, decimal>>();
            Trades = new List<Tuple<decimal, decimal>>();
            foreach (OrderBookEntryMessage trade in orderBookUpdatedMessage.Trades)
            {
                decimal price = DWConverter.FromProtoDecimal(trade.Price);
                decimal quantity = DWConverter.FromProtoDecimal(trade.Quantity);
                Tuple<decimal, decimal> tuple = new Tuple<decimal, decimal>(price, quantity);
                Trades.Add(tuple);
            }
            foreach (OrderBookEntryMessage ask in orderBookUpdatedMessage.AskUpdates)
            {
                decimal price = DWConverter.FromProtoDecimal(ask.Price);
                decimal quantity = DWConverter.FromProtoDecimal(ask.Quantity);
                Tuple<decimal, decimal> tuple = new Tuple<decimal, decimal>(price, quantity);
                AskUpdates.Add(tuple);
            }
            foreach (OrderBookEntryMessage bid in orderBookUpdatedMessage.BidUpdates)
            {
                decimal price = DWConverter.FromProtoDecimal(bid.Price);
                decimal quantity = DWConverter.FromProtoDecimal(bid.Quantity);
                Tuple<decimal, decimal> tuple = new Tuple<decimal, decimal>(price, quantity);
                BidUpdates.Add(tuple);
            }
            LastFullTimestamp = DWConverter.FromLongDateTime(orderBookUpdatedMessage.LastFullTimestamp);
            LastTradePrice = DWConverter.FromProtoDecimal(orderBookUpdatedMessage.LastTradePrice);
            LastTradeQuantity = DWConverter.FromProtoDecimal(orderBookUpdatedMessage.LastTradeQuantity);
            MarkPrice = DWConverter.FromProtoDecimal(orderBookUpdatedMessage.MarkPrice);
            UpdateSerial = orderBookUpdatedMessage.UpdateSerial;
        }
    }
}
