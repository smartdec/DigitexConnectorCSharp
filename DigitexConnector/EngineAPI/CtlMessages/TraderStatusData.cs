using System.Collections.Generic;
using DigitexConnector.Orders;
using DigitexWire;
using DigitexConnector.Extentions;

namespace DigitexConnector.EngineAPI
{  
   /// <summary>
   /// Trader status message
   /// </summary>
   /// <inheritdoc/>
    public class TraderStatusData : CommonFields
    {
        /// <summary>
        /// Spot price.
        /// </summary>
        public decimal MarkPrice { get; }

        /// <summary>
        /// Orders which are placed by the trader.
        /// </summary>
        /// <see cref="OrderBase"/>
        public List<OrderBase> Orders { get; }

        /// <summary>
        /// Trades which are placed by the trader.
        /// </summary>
        /// <see cref="Trade"/>
        public List<Trade> Trades { get; }

        /// <summary>
        /// Price of last trade from all trades of trader.
        /// </summary>
        public decimal LastTradePrice { get; }

        /// <summary>
        /// Quantity of last trade from all trades of all traders.
        /// </summary>
        public decimal LastTradeQuantity { get; }

        /// <summary>
        /// The value at which bankruptcy occurs.
        /// </summary>
        public decimal PositionBankruptcyVolume { get; }

        /// <summary>
        /// Number of contracts in position.
        /// </summary>
        public decimal PositionContracts { get; }

        /// <summary>
        /// Value when liquidation must be called.
        /// </summary>
        public decimal PositionLiquidationVolume { get; }

        /// <summary>
        /// Current position of trader.
        /// </summary>
        public OrderPosition PositionType { get; }

        /// <summary>
        /// Volume of position.
        /// </summary>
        public decimal PositionVolume { get; }

        /// <summary>
        /// Current leverage.
        /// </summary>
        public uint Leverage { get; }

        public uint MarketId { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="Message"/>
        /// <param name="message">Source message.</param>
        public TraderStatusData(Message message)
        {
            MarketId = message.MarketId;
            TraderStatusMessage traderStatusMessage = message.TraderStatusMsg.Clone();
            TraderId = message.TraderId;
            Symbol = SymbolsContainer.GetSymbol(message.MarketId);
            Upnl = DWConverter.FromProtoDecimal(traderStatusMessage.Upnl);
            Pnl = DWConverter.FromProtoDecimal(traderStatusMessage.Pnl);
            AccumQuantity = DWConverter.FromProtoDecimal(traderStatusMessage.AccumQuantity);
            MarkPrice = DWConverter.FromProtoDecimal(traderStatusMessage.MarkPrice);
            Orders = new List<OrderBase>();
            foreach (OrderMessage orderMessage in traderStatusMessage.Orders)
            {
                OrderBase order = orderMessage.OrderType == OrderType.Market ? (OrderBase)new OrderMarket(orderMessage, SymbolsContainer.GetSymbol(message.MarketId)) :
                                  orderMessage.OrderType == OrderType.Limit ? (OrderBase)new OrderLimit(orderMessage, SymbolsContainer.GetSymbol(message.MarketId)) :
                                  null;

                order.Status = OrderStatus.Accepted;

                Orders.Add(order);
            }
            Trades = new List<Trade>();
            foreach (TradeMessage tradeMessage in traderStatusMessage.Trades)
            {
                Trade trade = new Trade(tradeMessage, Symbol);
                if (tradeMessage.TradeTimestamp == 0)
                {
                    trade.TradeTimeStamp = DWConverter.FromLongDateTime(message.Timestamp);
                }
                this.Trades.Add(trade);
            }
            Leverage = traderStatusMessage.Leverage;
            BuyOrderQuantity = DWConverter.FromProtoDecimal(traderStatusMessage.BuyOrderQuantity);
            SellOrderQuantity = DWConverter.FromProtoDecimal(traderStatusMessage.SellOrderQuantity);
            TraderBalance = DWConverter.FromProtoDecimal(traderStatusMessage.TraderBalance);
            PositionMargin = DWConverter.FromProtoDecimal(traderStatusMessage.PositionMargin);
            OrderMargin = DWConverter.FromProtoDecimal(traderStatusMessage.OrderMargin);
            BuyOrderMargin = DWConverter.FromProtoDecimal(traderStatusMessage.BuyOrderMargin);
            LastTradePrice = DWConverter.FromProtoDecimal(traderStatusMessage.LastTradePrice);
            LastTradeQuantity = DWConverter.FromProtoDecimal(traderStatusMessage.LastTradeQuantity);
            PositionBankruptcyVolume = DWConverter.FromProtoDecimal(traderStatusMessage.PositionBankruptcyVolume);
            PositionContracts = DWConverter.FromProtoDecimal(traderStatusMessage.PositionContracts);
            PositionLiquidationVolume = DWConverter.FromProtoDecimal(traderStatusMessage.PositionLiquidationVolume);
            PositionType = traderStatusMessage.PositionType;
            PositionVolume = DWConverter.FromProtoDecimal(traderStatusMessage.PositionVolume);
            SellOrderMargin = DWConverter.FromProtoDecimal(traderStatusMessage.SellOrderMargin);
        }
    }
}
