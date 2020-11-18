using System.Collections.Generic;
using DigitexWire;
using DigitexConnector.Extentions;
using DigitexConnector.Trading;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Data of market state updated message.
    /// </summary>
    public class MarketStateUpdateData : MarketStateCommonFields
    {
        /// <summary>
        /// Ratio coefficient of Digitex to base price. 
        /// </summary>
        public decimal DgtxToBasePrice { get; }

        /// <summary>
        /// Count of future.
        /// </summary>
        public decimal FutureCount { get; }

        /// <summary>
        /// Value of future.
        /// </summary>
        public decimal FutureValue { get; }

        /// <summary>
        /// Impact of ask count.
        /// </summary>
        public decimal ImpactAskCount { get; }

        /// <summary>
        /// Impact of ask value.
        /// </summary>
        public decimal ImpactAskValue { get; }

        /// <summary>
        /// Impact of base quantity.
        /// </summary>
        public decimal ImpactBaseQuantity { get; }

        /// <summary>
        /// Impact of bid count.
        /// </summary>
        public decimal ImpactBidCount { get; }

        /// <summary>
        /// Impact of bid value.
        /// </summary>
        public decimal ImpactBidValue { get; }

        /// <summary>
        /// Impact of quantity.
        /// </summary>
        public decimal ImpactQuantity { get; }

        /// <summary>
        /// Impact of value.
        /// </summary>
        public decimal ImpactValue { get; }

        /// <summary>
        /// Spot.
        /// </summary>
        public decimal SpotPrice { get; }

        public Symbol Symbol { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="Message"/>
        /// <param name="message">Source message.</param>
        public MarketStateUpdateData(Message message)
        {
            Symbol = SymbolsContainer.GetSymbol(message.MarketId);
            MarketStateUpdateMessage marketStateUpdateMessage = message.MarketStateUpdateMsg.Clone();
            MarketId = message.MarketId;
            Symbol symbol = SymbolsContainer.GetSymbol(MarketId);
            DailyStats = new List<DailyStatisticsData>();
            foreach (DailyStatisticsMessage dailyStats in marketStateUpdateMessage.DailyStats)
            {
                DailyStatisticsData stats = new DailyStatisticsData(dailyStats);
                DailyStats.Add(stats);
            }
            DgtxToBasePrice = DWConverter.FromProtoDecimal(marketStateUpdateMessage.DgtxToBasePrice);
            FundingInterval = marketStateUpdateMessage.FundingInterval;
            PayoutPerContract = DWConverter.FromProtoDecimal(marketStateUpdateMessage.PayoutPerContract);
            EventTimestamp = DWConverter.FromLongDateTime(marketStateUpdateMessage.EventTimestamp);
            FundingRate = DWConverter.FromProtoDecimal(marketStateUpdateMessage.FundingRate);
            FundingTime = marketStateUpdateMessage.FundingTime;
            FutureCount = DWConverter.FromProtoDecimal(marketStateUpdateMessage.FutureCount);
            FutureValue = DWConverter.FromProtoDecimal(marketStateUpdateMessage.FutureValue);
            ImpactAskCount = DWConverter.FromProtoDecimal(marketStateUpdateMessage.ImpactAskCount);
            ImpactAskValue = DWConverter.FromProtoDecimal(marketStateUpdateMessage.ImpactAskValue);
            ImpactBaseQuantity = DWConverter.FromProtoDecimal(marketStateUpdateMessage.ImpactBaseQuantity);
            ImpactBidCount = DWConverter.FromProtoDecimal(marketStateUpdateMessage.ImpactBidCount);
            ImpactBidValue = DWConverter.FromProtoDecimal(marketStateUpdateMessage.ImpactBidValue);
            ImpactQuantity = DWConverter.FromProtoDecimal(marketStateUpdateMessage.ImpactQuantity);
            ImpactValue = DWConverter.FromProtoDecimal(marketStateUpdateMessage.ImpactValue);
            LastTradePrice = DWConverter.FromProtoDecimal(marketStateUpdateMessage.LastTradePrice);
            LastTradeQuantity = DWConverter.FromProtoDecimal(marketStateUpdateMessage.LastTradeQuantity);
            Ohlcvs = new List<OHLCVData>();
            foreach (OHLCVMessage ohlcv in marketStateUpdateMessage.Ohlcvs)
            {
                OHLCVData ohlc = new OHLCVData(ohlcv);
                Ohlcvs.Add(ohlc);
            }
            SpotPrice = DWConverter.FromProtoDecimal(marketStateUpdateMessage.SpotPrice);
            Trades = new List<Trade>();
            foreach (TradeMessage trade in marketStateUpdateMessage.Trades)
            {
                Trade tTrade = new Trade(trade, symbol);
                Trades.Add(tTrade);
            }
        }
    }
}
