namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Common fields for all meassages received in Executor via data channel.
    /// </summary>
    public class OrderBookCommonFields
    {
        /// <summary>
        /// Price of last trade from all trades of all traders.
        /// </summary>
        public decimal LastTradePrice { get; protected set; }

        /// <summary>
        /// Quantity of last trade from all trades of all traders.
        /// </summary>
        public decimal LastTradeQuantity { get; protected set; }

        /// <summary>
        /// Spot price.
        /// </summary>
        public decimal MarkPrice { get; protected set; }

        /// <summary>
        /// Current market id.
        /// </summary>
        public uint MarketId { get; protected set; }

        public Symbol Symbol { get; protected set; }
    }
}
