using System;
using System.Collections.Generic;
using DigitexWire;
using DigitexConnector.Extentions;
using DigitexConnector.Trading;

namespace DigitexConnector.EngineAPI
{
    ///<summary>
    ///Order filled status message.
    ///</summary>
    /// <inheritdoc/>
    public class OrderFilledData : CommonFields
    {
        /// <summary>
        /// Order id.
        /// </summary>
        public Guid NewClientId { get; }

        /// <summary>
        /// Not completed part of market order`s quantity.
        /// </summary>
        public decimal DroppedQuantity { get; }

        /// <summary>
        /// Price of last trade from all trades of all traders.
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
        /// Volume of position.
        /// </summary>
        public decimal PositionVolume { get; }

        /// <summary>
        /// Trades executed at one time.
        /// </summary>
        /// <see cref="Trade"/>
        public List<Trade> RawTrades { get; }

        /// <summary>
        /// All trades.
        /// </summary>
        /// <see cref="Trade"/>
        public List<Trade> Trades { get; }

        /// <summary>
        /// Status of order.
        /// </summary>
        public OrderStatus Status { get; }

        /// <summary>
        /// Order price.
        /// </summary>
        public decimal Price { get; }

        /// <summary>
        /// Order quantity.
        /// </summary>
        public decimal Quantity { get; }

        /// <summary>
        /// Order leverage.
        /// </summary>
        public uint Leverage { get; }

        /// <summary>
        /// Type of order.
        /// </summary>
        public OrderType OrderType { get; }

        /// <summary>
        /// Duration of order.
        /// </summary>
        public OrderDuration Duration { get; }

        /// <summary>
        /// Paid price of order.
        /// </summary>
        public decimal PaidPrice { get; }

        /// <summary>
        /// Spot price.
        /// </summary>
        public decimal MarkPrice { get; }

        /// <summary>
        /// Type of current position.
        /// </summary>
        public OrderPosition PositionType { get; }

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
        public OrderFilledData(Message message)
        {
            OrderFilledMessage orderFilledMessage = message.OrderFilledMsg.Clone();
            TraderId = message.TraderId;
            ClientId = DWConverter.FromProtoUuid(message.ClientId);
            Symbol = SymbolsContainer.GetSymbol(message.MarketId);
            Status = orderFilledMessage.Status;
            AccumQuantity = DWConverter.FromProtoDecimal(orderFilledMessage.AccumQuantity);
            BuyOrderQuantity = DWConverter.FromProtoDecimal(orderFilledMessage.BuyOrderQuantity);
            SellOrderQuantity = DWConverter.FromProtoDecimal(orderFilledMessage.SellOrderQuantity);
            PositionType = orderFilledMessage.PositionType;
            PositionMargin = DWConverter.FromProtoDecimal(orderFilledMessage.PositionMargin);
            OrderMargin = DWConverter.FromProtoDecimal(orderFilledMessage.OrderMargin);
            TraderBalance = DWConverter.FromProtoDecimal(orderFilledMessage.TraderBalance);
            TraderBalance2 = DWConverter.FromProtoDecimal(orderFilledMessage.TraderBalance2);
            Pnl = DWConverter.FromProtoDecimal(orderFilledMessage.Pnl);
            Upnl = DWConverter.FromProtoDecimal(orderFilledMessage.Upnl);
            NewClientId = Status == OrderStatus.Partial ? DWConverter.FromProtoUuid(orderFilledMessage.NewClientId) : Guid.Empty;
            BuyOrderMargin = DWConverter.FromProtoDecimal(orderFilledMessage.BuyOrderMargin);
            DroppedQuantity = DWConverter.FromProtoDecimal(orderFilledMessage.DroppedQuantity);
            Duration = orderFilledMessage.Duration;
            LastTradePrice = DWConverter.FromProtoDecimal(orderFilledMessage.LastTradePrice);
            LastTradeQuantity = DWConverter.FromProtoDecimal(orderFilledMessage.LastTradeQuantity);
            Leverage = orderFilledMessage.Leverage;
            MarkPrice = DWConverter.FromProtoDecimal(orderFilledMessage.MarkPrice);
            OrderType = orderFilledMessage.OrderType;
            PaidPrice = DWConverter.FromProtoDecimal(orderFilledMessage.PaidPrice);
            PositionBankruptcyVolume = DWConverter.FromProtoDecimal(orderFilledMessage.PositionBankruptcyVolume);
            PositionContracts = DWConverter.FromProtoDecimal(orderFilledMessage.PositionContracts);
            PositionLiquidationVolume = DWConverter.FromProtoDecimal(orderFilledMessage.PositionLiquidationVolume);
            PositionVolume = DWConverter.FromProtoDecimal(orderFilledMessage.PositionVolume);
            Price = DWConverter.FromProtoDecimal(orderFilledMessage.Price);
            Quantity = DWConverter.FromProtoDecimal(orderFilledMessage.Quantity);

            RawTrades = new List<Trade>();
            foreach (TradeMessage trade in orderFilledMessage.RawTrades)
            {
                Trade rawTrade = new Trade(trade, SymbolsContainer.GetSymbol(message.MarketId));
                if (trade.TradeTimestamp == 0)
                {
                    rawTrade.TradeTimeStamp = DWConverter.FromLongDateTime(message.Timestamp);
                }
                RawTrades.Add(rawTrade);
            }
            SellOrderMargin = DWConverter.FromProtoDecimal(orderFilledMessage.SellOrderMargin);
            Side = orderFilledMessage.Side;
            Trades = new List<Trade>();
            foreach (TradeMessage trade in orderFilledMessage.Trades)
            {
                Trade tempTrade = new Trade(trade, SymbolsContainer.GetSymbol(message.MarketId));
                if (trade.TradeTimestamp == 0)
                {
                    tempTrade.TradeTimeStamp = DWConverter.FromLongDateTime(message.Timestamp);
                }
                Trades.Add(tempTrade);
            }
            OpenTime = DWConverter.FromLongDateTime(orderFilledMessage.OpenTime);
            OrigClientId = DWConverter.FromProtoUuid(orderFilledMessage.OrigClientId);
            OrigQuantity = DWConverter.FromProtoDecimal(orderFilledMessage.OrigQuantity);
        }
    }
}
