using DigitexWire;
using DigitexConnector.EngineAPI;

namespace DigitexConnector.Trading
{
    /// <summary>
    /// Common indexec of trader from exchange.
    /// </summary>
    public class TraderInfo
    {
        public static decimal TraderBalance { private set; get; }
        public decimal Pnl { private set; get; }
        public decimal Upnl { private set; get; }
        public decimal OrderMargin { private set; get; }
        public readonly Symbol Symbol;
        public decimal PositionMargin { private set; get; }
        public decimal PositionContracts { private set; get; }
        public OrderPosition PositionType { private set; get; }
        public decimal BuyOrderMargin { private set; get; }
        public decimal LastTradePrice { private set; get; }
        public decimal LastTradeQuantity { private set; get; }
        public decimal PositionLiquidationVolume { private set; get; }
        public decimal PositionVolume { private set; get; }
        public decimal SellOrderMargin { private set; get; }
        public decimal AccumQuantity { private set; get; }
        public uint Leverage { private set; get; }
        public decimal BuyOrderQuantity { private set; get; }
        public decimal SellOrderQuantity { private set; get; }
        public decimal AvailableBalance => TraderBalance - OrderMargin - PositionMargin;

        public TraderInfo(Symbol symbol)
        {
            Symbol = symbol;
        }

        private void Update(CommonFields data)
        {
            AccumQuantity = data.AccumQuantity;
            BuyOrderMargin = data.BuyOrderMargin;
            BuyOrderQuantity = data.BuyOrderQuantity;
            OrderMargin = data.OrderMargin;
            Pnl = data.Pnl ?? 0;
            PositionMargin = data.PositionMargin;
            SellOrderMargin = data.SellOrderMargin;
            SellOrderQuantity = data.SellOrderQuantity;
            TraderBalance = data.TraderBalance;
            Upnl = data.Upnl ?? 0;
        }

        internal void Update(OrderStatusData data) => Update(data as CommonFields);

        internal void Update(OrderFilledData data)
        {
            Update(data as CommonFields);
            LastTradePrice = data.LastTradePrice;
            LastTradeQuantity = data.LastTradeQuantity;
            PositionContracts = data.PositionContracts;
            PositionLiquidationVolume = data.PositionLiquidationVolume;
            PositionType = data.PositionType;
            PositionVolume = data.PositionVolume;
        }

        internal void Update(TraderBalanceData data)
        {
            TraderBalance = data.TraderBalance;
            PositionMargin = data.PositionMargin;
            OrderMargin = data.OrderMargin;
        }

        internal void Update(TraderStatusData data)
        {
            Update(data as CommonFields);
            LastTradePrice = data.LastTradePrice;
            LastTradeQuantity = data.LastTradeQuantity;
            Leverage = data.Leverage;
            PositionContracts = data.PositionContracts;
            PositionLiquidationVolume = data.PositionLiquidationVolume;
            PositionType = data.PositionType;
            PositionVolume = data.PositionVolume;
        }

        internal void Update(FundingData data)
        {
            Update(data as CommonFields);
            LastTradePrice = data.LastTradePrice;
            LastTradeQuantity = data.LastTradeQuantity;
            PositionContracts = data.PositionContracts;
            PositionLiquidationVolume = data.PositionLiquidationVolume;
            PositionType = data.PositionType;
            PositionVolume = data.PositionVolume;
        }

        internal void Update(LeverageData data)
        {
            Update(data as CommonFields);
            LastTradePrice = data.LastTradePrice;
            LastTradeQuantity = data.LastTradeQuantity;
            Leverage = data.Leverage;
            PositionContracts = data.PositionContracts;
            PositionLiquidationVolume = data.PositionLiquidationVolume;
            PositionType = data.PositionType;
            PositionVolume = data.PositionVolume;
        }

        internal void Update(OrderCanceledData data) => Update(data as CommonFields);
    }
}
