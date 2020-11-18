using System;
using DigitexWire;
using DigitexConnector.Extentions;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Data of OHLCV message (candle).
    /// </summary>
    public class OHLCVData
    {
        /// <summary>
        /// Time stamp of candle.
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// Open price.
        /// </summary>
        public decimal OpenPrice { get; }

        /// <summary>
        /// High price.
        /// </summary>
        public decimal HighPrice { get; }

        /// <summary>
        /// Low price.
        /// </summary>
        public decimal LowPrice { get; }

        /// <summary>
        /// Close price.
        /// </summary>
        public decimal ClosePrice { get; }

        /// <summary>
        /// Average price of spot.
        /// </summary>
        public decimal AverageOraclePrice { get; }

        /// <summary>
        /// Volume of candle.
        /// </summary>
        public decimal Volume { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="OHLCVMessage"/>
        /// <param name="message">Source message.</param>
        public OHLCVData(OHLCVMessage message)
        {
            TimeStamp = DWConverter.FromLongDateTime(message.Timestamp);
            OpenPrice = DWConverter.FromProtoDecimal(message.OpenPrice);
            HighPrice = DWConverter.FromProtoDecimal(message.HighPrice);
            LowPrice = DWConverter.FromProtoDecimal(message.LowPrice);
            ClosePrice = DWConverter.FromProtoDecimal(message.ClosePrice);
            AverageOraclePrice = DWConverter.FromProtoDecimal(message.AverageOraclePrice);
            Volume = DWConverter.FromProtoDecimal(message.Volume);
        }
    }
}
