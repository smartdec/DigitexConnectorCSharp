using System;
using OrderPosition = DigitexWire.OrderPosition;
using DigitexWire;
using DigitexConnector.Extentions;
using DigitexConnector.EngineAPI;

namespace DigitexConnector
{
    /// <summary>
    /// Contains information about performed trade
    /// </summary>
    public class Trade
    {
        private Guid oldClientId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Original message. <see cref="TradeMessage"/></param>
        public Trade(TradeMessage message, Symbol symbol)
        {
            TradeTraderId = message.TradeTraderId;
            TradeTimeStamp = DWConverter.FromLongDateTime(message.TradeTimestamp);
            Position = message.Position;
            Price = DWConverter.FromProtoDecimal(message.Price);
            Quantity = DWConverter.FromProtoDecimal(message.Quantity);
            PaidPrice = DWConverter.FromProtoDecimal(message.PaidPrice);
            LiquidationPrice = DWConverter.FromProtoDecimal(message.LiquidationPrice);
            ExitPrice = DWConverter.FromProtoDecimal(message.ExitPrice);
            Leverage = message.Leverage;
            ContractId = message.ContractId;
            OldContractId = message.OldContractId;
            OldClientId = DWConverter.FromProtoUuid(message.OldClientId);
            IsIncrease = message.IsIncrease;
            IsLiquidation = message.IsLiquidation;
            Symbol = symbol;
        }

        public Trade()
        { }

        public uint TradeTraderId { internal set; get; }

        public DateTime TradeTimeStamp { internal set; get; }

        public OrderPosition Position { internal set; get; }

        public decimal Price { internal set; get; }

        public decimal Quantity { internal set; get; }

        public decimal PaidPrice { internal set; get; }

        public decimal LiquidationPrice { internal set; get; }

        public decimal ExitPrice { internal set; get; }

        public uint Leverage { internal set; get; }

        public ulong ContractId { internal set; get; }

        public ulong OldContractId { internal set; get; }

        public int IsIncrease { internal set; get; }

        public int IsLiquidation { internal set; get; }

        public Guid OldClientId
        {
            internal set { oldClientId = value; }
            get { return oldClientId; }
        }

        public Symbol Symbol { internal set; get; }
    }
}
