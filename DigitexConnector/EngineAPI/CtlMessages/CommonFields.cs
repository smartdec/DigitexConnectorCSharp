using System;

namespace DigitexConnector.EngineAPI
{
    /// <summary>
    /// Common fields for all meassages received in Executor via ctl channel.
    /// </summary>
    public class CommonFields
    {
        /// <summary>
        /// Id of specidif trader.
        /// </summary>
        public uint TraderId { get; protected set; }
        /// <summary>
        /// Order id.
        /// </summary>
        public Guid ClientId { get; protected set; }
        /// <summary>
        /// Symbol.
        /// </summary>
        public Symbol Symbol { get; protected set; }
        /// <summary>
        /// Margin of position.
        /// </summary>
        public decimal PositionMargin { get; protected set; }
        /// <summary>
        /// Margin of all orders.
        /// </summary>
        public decimal OrderMargin { get; protected set; }
        /// <summary>
        /// Balance of trader.
        /// </summary>
        public decimal TraderBalance { get; protected set; }
        /// <summary>
        /// Unrealized profit and loss.
        /// </summary>
        public decimal? Upnl { get; protected set; }
        /// <summary>
        /// Realized profit and loss.
        /// </summary>
        public decimal? Pnl { get; protected set; }
        /// <summary>
        /// Margin of buy orders.
        /// </summary>
        public decimal BuyOrderMargin { get; protected set; }
        /// <summary>
        /// Margin of buy orders.
        /// </summary>
        public decimal SellOrderMargin { get; protected set; }
        /// <summary>
        /// Accum quantity.
        /// </summary>
        public decimal AccumQuantity { get; protected set; }
        /// <summary>
        /// Quantity of all active long orders.
        /// </summary>
        public decimal BuyOrderQuantity { get; protected set; }
        /// <summary>
        /// Quantity of all active short orders.
        /// </summary>
        public decimal SellOrderQuantity { get; protected set; }
    }

}
