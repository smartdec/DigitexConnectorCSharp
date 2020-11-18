using System.Collections.Generic;
using DigitexWire;
using DigitexConnector.Extentions;
using DigitexConnector.Trading;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Funding message.
    /// </summary>
    /// <inheritdoc/>
    public class FundingData : CommonFields
    {
        /// <summary>
        /// Spot price.
        /// </summary>
        public decimal MarkPrice { get; }

        /// <summary>
        /// Price of last trade from all trades of all traders.
        /// </summary>
        public decimal LastTradePrice { get; }

        /// <summary>
        /// Quantity of last trade from all trades of all traders.
        /// </summary>
        public decimal LastTradeQuantity { get; }

        /// <summary>
        /// Contract payout.
        /// </summary>
        public decimal PayoutPerContract { get; }

        /// <summary>
        /// List of trades.
        /// </summary>
        public List<Trade> Trades { get; }

        /// <summary>
        /// Position of all contracts.
        /// </summary>
        public decimal PositionContracts { get; }

        /// <summary>
        /// Volume of position.
        /// </summary>
        public decimal PositionVolume { get; }

        /// <summary>
        /// Volume of liquidation current position.
        /// </summary>
        public decimal PositionLiquidationVolume { get; }

        /// <summary>
        /// Volumme of bankruptcy current position.
        /// </summary>
        public decimal PositionBankruptcyVolume { get; }

        /// <summary>
        /// Payout.
        /// </summary>
        public decimal Payout { get; }

        /// <summary>
        /// Type of current position.
        /// </summary>
        public OrderPosition PositionType { get; }

        /// <summary>
        /// Position margin change.
        /// </summary>
        public decimal PositionMarginChange { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="Message"/>
        /// <param name="message">Source message.</param>
        public FundingData(Message message)
        {
            FundingMessage fundingMessage = message.FundingMsg.Clone();
            TraderId = message.TraderId;
            Symbol = SymbolsContainer.GetSymbol(message.MarketId);
            MarkPrice = DWConverter.FromProtoDecimal(fundingMessage.MarkPrice);
            OrderMargin = DWConverter.FromProtoDecimal(fundingMessage.OrderMargin);
            PositionMargin = DWConverter.FromProtoDecimal(fundingMessage.PositionMargin);
            TraderBalance = DWConverter.FromProtoDecimal(fundingMessage.TraderBalance);
            Upnl = DWConverter.FromProtoDecimal(fundingMessage.Upnl);
            Pnl = DWConverter.FromProtoDecimal(fundingMessage.Pnl);
            AccumQuantity = DWConverter.FromProtoDecimal(fundingMessage.AccumQuantity);
            BuyOrderMargin = DWConverter.FromProtoDecimal(fundingMessage.BuyOrderMargin);
            LastTradePrice = DWConverter.FromProtoDecimal(fundingMessage.LastTradePrice);
            LastTradeQuantity = DWConverter.FromProtoDecimal(fundingMessage.LastTradeQuantity);
            PayoutPerContract = DWConverter.FromProtoDecimal(fundingMessage.PayoutPerContract);
            SellOrderMargin = DWConverter.FromProtoDecimal(fundingMessage.SellOrderMargin);
            Trades = new List<Trade>();
            foreach (TradeMessage tradeMessage in fundingMessage.Trades)
            {
                Trade trade = new Trade(tradeMessage, SymbolsContainer.GetSymbol(message.MarketId));
                if (tradeMessage.TradeTimestamp == 0)
                {
                    trade.TradeTimeStamp = DWConverter.FromLongDateTime(message.Timestamp);
                }
                Trades.Add(trade);
            }
            PositionContracts = DWConverter.FromProtoDecimal(fundingMessage.PositionContracts);
            PositionVolume = DWConverter.FromProtoDecimal(fundingMessage.PositionVolume);
            PositionLiquidationVolume = DWConverter.FromProtoDecimal(fundingMessage.PositionLiquidationVolume);
            PositionBankruptcyVolume = DWConverter.FromProtoDecimal(fundingMessage.PositionBankruptcyVolume);
            BuyOrderQuantity = DWConverter.FromProtoDecimal(fundingMessage.BuyOrderQuantity);
            SellOrderQuantity = DWConverter.FromProtoDecimal(fundingMessage.SellOrderQuantity);
            Payout = DWConverter.FromProtoDecimal(fundingMessage.Payout);
            PositionType = fundingMessage.PositionType;
            PositionMarginChange = DWConverter.FromProtoDecimal(fundingMessage.PositionMarginChange);
        }
    }
}
