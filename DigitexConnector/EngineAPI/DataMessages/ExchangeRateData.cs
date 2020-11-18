using DigitexWire;
using DigitexConnector.Extentions;

namespace DigitexConnector.EngineAPI
{
    public class ExchangeRateData
    {
        /// <summary>
        /// Id of currency pair.
        /// </summary>
        public uint CurrencyPairId { get; }

        /// <summary>
        /// Spot price of current pair.
        /// </summary>
        public decimal MarkPrice { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="Message"/>
        /// <param name="message">Source message.</param>
        public ExchangeRateData(Message message)
        {
            ExchangeRateMessage exchangeRateMessage = message.ExchangeRateMsg.Clone();
            CurrencyPairId = exchangeRateMessage.CurrencyPairId;
            MarkPrice = DWConverter.FromProtoDecimal(exchangeRateMessage.MarkPrice);
        }

    }
}
