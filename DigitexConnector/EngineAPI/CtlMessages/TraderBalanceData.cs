using DigitexWire;
using DigitexConnector.Extentions;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Trader balance message
    /// </summary>
    /// <inheritdoc/>
    public class TraderBalanceData
    {
        public uint TraderId { get; }
        public uint CurrencyId { get; }
        public Symbol Symbol { get; }
        public decimal TraderBalance { get; }
        public decimal PositionMargin { get; }
        public decimal OrderMargin { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <see cref="Message"/>
        /// <param name="message">Source message.</param>
        public TraderBalanceData(Message message)
        {
            TraderBalanceMessage traderBalanceMessage = message.TraderBalanceMsg.Clone();
            TraderId = message.TraderId;
            Symbol = SymbolsContainer.GetSymbol(message.MarketId);
            CurrencyId = traderBalanceMessage.CurrencyId;
            OrderMargin = DWConverter.FromProtoDecimal(traderBalanceMessage.OrderMargin);
            PositionMargin = DWConverter.FromProtoDecimal(traderBalanceMessage.PositionMargin);
            TraderBalance = DWConverter.FromProtoDecimal(traderBalanceMessage.TraderBalance);
        }
    }
}
