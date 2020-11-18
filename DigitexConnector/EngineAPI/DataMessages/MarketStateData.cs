using System.Collections.Generic;
using DigitexWire;
using DigitexConnector.Extentions;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Data of market state message.
    /// </summary>
    public class MarketStateData : MarketStateCommonFields
    {
        /// <summary>
        /// Next funding rate.
        /// </summary>
        public decimal NextFundingRate { get; }

        /// <summary>
        /// Value of one contract.
        /// </summary>
        public decimal ContractValue { get; }

        /// <summary>
        /// Price of one tick.
        /// </summary>
        public decimal TickPrice { get; }

        /// <summary>
        /// Value of one tick.
        /// </summary>
        public decimal TickValue { get; }

        public Symbol Symbol { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="Message"/>
        /// <param name="message">Surce message.</param>
        public MarketStateData(Message message)
        {
            Symbol = SymbolsContainer.GetSymbol(message.MarketId);
            MarketStateMessage marketStateMessage = message.MarketStateMsg.Clone();
            MarketId = message.MarketId;
            ContractValue = DWConverter.FromProtoDecimal(marketStateMessage.ContractValue);
            DailyStats = new List<DailyStatisticsData>();
            foreach (DailyStatisticsMessage dailyStats in marketStateMessage.DailyStats)
            {
                DailyStatisticsData stats = new DailyStatisticsData(dailyStats);
                DailyStats.Add(stats);
            }
            EventTimestamp = DWConverter.FromLongDateTime(marketStateMessage.EventTimestamp);
            FundingRate = DWConverter.FromProtoDecimal(marketStateMessage.FundingRate);
            FundingTime = marketStateMessage.FundingTime;
            LastTradePrice = DWConverter.FromProtoDecimal(marketStateMessage.LastTradePrice);
            LastTradeQuantity = DWConverter.FromProtoDecimal(marketStateMessage.LastTradeQuantity);
            NextFundingRate = DWConverter.FromProtoDecimal(marketStateMessage.NextFundingRate);
            Ohlcvs = new List<OHLCVData>();
            foreach (OHLCVMessage ohlcv in marketStateMessage.Ohlcvs)
            {
                OHLCVData ohlc = new OHLCVData(ohlcv);
                Ohlcvs.Add(ohlc);
            }
            TickPrice = DWConverter.FromProtoDecimal(marketStateMessage.TickPrice);
            TickValue = DWConverter.FromProtoDecimal(marketStateMessage.TickValue);
            Trades = new List<Trade>();
            foreach (TradeMessage trade in marketStateMessage.Trades)
            {
                Trade tTrade = new Trade(trade, SymbolsContainer.GetSymbol(MarketId));
                Trades.Add(tTrade);
            }
            FundingInterval = marketStateMessage.FundingInterval;
            PayoutPerContract = DWConverter.FromProtoDecimal(marketStateMessage.PayoutPerContract);
        }
    }
}
