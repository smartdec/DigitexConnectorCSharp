using System;
using DigitexWire;
using DigitexConnector.Extentions;

namespace DigitexConnector.EngineAPI
{
    public class DailyStatisticsData
    {
        /// <summary>
        /// Gross.
        /// </summary>
        public decimal Gross { get; }

        /// <summary>
        /// Daily high price.
        /// </summary>
        public decimal HighPrice { get; }

        /// <summary>
        /// Daily low price.
        /// </summary>
        public decimal LowPrice { get; }

        /// <summary>
        /// Time of daily start.
        /// </summary>
        public DateTime StartTimestamp { get; }

        /// <summary>
        /// Daily volume.
        /// </summary>
        public decimal Volume { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <see cref="DailyStatisticsMessage"/>
        /// <param name="message">Source message.</param>
        public DailyStatisticsData(DailyStatisticsMessage message)
        {

            Gross = DWConverter.FromProtoDecimal(message.Gross);
            HighPrice = DWConverter.FromProtoDecimal(message.HighPrice);
            LowPrice = DWConverter.FromProtoDecimal(message.LowPrice);
            StartTimestamp = DWConverter.FromLongDateTime(message.StartTimestamp);
            Volume = DWConverter.FromProtoDecimal(message.Volume);
        }
    }
}
