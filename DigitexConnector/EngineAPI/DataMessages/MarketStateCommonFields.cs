using System;
using System.Collections.Generic;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Common fields of market state objects.
    /// </summary>
    public class MarketStateCommonFields
    {
        /// <summary>
        /// Spot.
        /// </summary>
        public uint MarketId { get; protected set; }

        /// <summary>
        /// Time of market state event.
        /// </summary>
        public DateTime EventTimestamp { get; protected set; }

        /// <summary>
        /// Rate of funding.
        /// </summary>
        public decimal FundingRate { get; protected set; }

        /// <summary>
        /// Next funding time.
        /// </summary>
        public long FundingTime { get; protected set; }

        /// <summary>
        /// Last trade price of current market id.
        /// </summary>
        public decimal LastTradePrice { get; protected set; }

        /// <summary>
        /// Last trade quantity of current market id.
        /// </summary>
        public decimal LastTradeQuantity { get; protected set; }

        /// <summary>
        /// <see cref="DailyStatisticsData"/>
        /// </summary>
        public List<DailyStatisticsData> DailyStats { get; protected set; }

        /// <summary>
        /// <see cref="OHLCVData"/>
        /// </summary>
        public List<OHLCVData> Ohlcvs { get; protected set; }

        /// <summary>
        /// <see cref="Trade"/>
        /// </summary>
        public List<Trade> Trades { get; protected set; }

        /// <summary>
        /// Current interval of funding.
        /// </summary>
        public long FundingInterval { get; protected set; }

        /// <summary>
        /// Current payout per contract.
        /// </summary>
        public decimal PayoutPerContract { get; protected set; }
    }
}
