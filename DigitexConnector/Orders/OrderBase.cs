using System;
using DigitexWire;
using DigitexConnector.EngineAPI;
using DigitexConnector.Extentions;
using System.Collections.Generic;
using System.Linq;
using DigitexConnector.Interfaces;

namespace DigitexConnector.Orders
{
    public abstract class OrderBase : IDisposable
    {
        /// <summary>
        /// Event that happens when status of order is switched.
        /// </summary>
        public Action<OrderBase> OrderStatusChanged;

        /// <summary>
        /// Event that happens when error related to order is received from exchange.
        /// </summary>
        public Action<OrderBase, ErrorCodes> ErrorEvent;

        private DigitexWire.OrderStatus? status;
        protected IAggregator Account;

        public readonly List<Trade> Trades = new List<Trade>();

        /// <summary>
        /// Symbol of order.
        /// </summary>
        public readonly Symbol TargetSymbol;

        protected OrderBase(OrderMessage message, Symbol symbol) 
        {
            ClientId = DWConverter.FromProtoUuid(message.OrderClientId);
            TimeStamp = DWConverter.FromLongDateTime(message.OrderTimestamp);
            Type = message.OrderType;
            Side = message.Side;
            Leverage = message.Leverage;
            Duration = message.Duration;
            Price = DWConverter.FromProtoDecimal(message.Price);
            Quantity = DWConverter.FromProtoDecimal(message.Quantity);
            PaidPrice = DWConverter.FromProtoDecimal(message.PaidPrice);
            status = null;
            TargetSymbol = symbol;
            ContractId = message.ContractId;
            OldClientId = DWConverter.FromProtoUuid(message.OldClientId);
            OpenTime = DWConverter.FromLongDateTime(message.OpenTime);
            OrderTraderId = message.OrderTraderId;
            OrigClientId = DWConverter.FromProtoUuid(message.OrigClientId);
            OrigQuantity = DWConverter.FromProtoDecimal(message.OrigQuantity);
        }

        protected OrderBase(OrderType type, OrderSide side, decimal quantity, IAggregator account, Symbol symbol,
            decimal price = 0, OrderDuration duration = OrderDuration.Gtc, uint leverage = 1)
        {
            Account = account;
            TargetSymbol = symbol;
            Type = type;
            ClientId = Guid.NewGuid();
            OrigClientId = ClientId;
            Quantity = quantity;
            if (quantity < 0)
            {
                throw new Exception("negative quantity order");
            }
            Side = side;
            Duration = duration;
            Leverage = leverage;
            Price = price;
            status = DigitexWire.OrderStatus.Pending;
        }

        protected OrderBase(OrderType type, OrderStatusData data, Symbol symbol)
        {
            Type = data.OrderType;
            ClientId = data.ClientId;
            Quantity = data.Quantity;
            if (Quantity < 0)
            {
                return;
            }
            Side = data.Side;
            Duration = data.Duration;
            Leverage = data.Leverage;
            Price = data.Price;
            TargetSymbol = symbol;
            PaidPrice = data.PaidPrice;
            AccumQuantity = data.AccumQuantity;
            OldContractId = data.OldContractId;
            OpenTime = data.OpenTime;
            OrderClientId = data.OrderClientId;
            TimeStamp = data.OrderTimestamp;
            OrigClientId = data.OrigClientId;
            OrigQuantity = data.OrigQuantity;
            Status = data.Status;
        }

        public void OnError(ErrorCodes errorCode)
        {
            ErrorEvent?.Invoke(this, errorCode);
        }

        /// <summary>
        /// Order's id.
        /// </summary>
        public Guid ClientId { internal set; get; }

        /// <summary>
        /// Orders type.
        /// </summary>
        public OrderType Type { internal set; get; }

        /// <summary>
        /// Time of order creation.
        /// </summary>
        public DateTime TimeStamp { internal set; get; }

        /// <summary>
        /// Order's quantity.
        /// </summary>
        public decimal Quantity { internal set; get; }

        /// <summary>
        /// Side of order.
        /// </summary>
        public OrderSide Side { internal set; get; }

        /// <summary>
        /// Order's status.
        /// </summary>
        public DigitexWire.OrderStatus? Status
        {
            internal set
            {
                if (value != status)
                {
                    status = value;
                }
            }
            get { return status; }
        }

        /// <summary>
        /// Duration of order.
        /// </summary>
        public OrderDuration Duration { internal set; get; }

        /// <summary>
        /// Order's price.
        /// </summary>
        public decimal Price { internal protected set; get; }

        public decimal PaidPrice { internal set; get; }

        public uint Leverage { internal set; get; }

        public decimal StopLossPrice { internal set; get; }

        public OrderType StopLossType { internal set; get; }

        public ulong ContractId { get; }

        public Guid OldClientId { internal set; get; }

        public DateTime OpenTime { internal set; get; }

        public uint OrderTraderId { get; }

        public Guid OrigClientId { internal set; get; }

        public decimal OrigQuantity { internal set; get; }

        public decimal FilledQuantity { internal set; get; }

        public decimal AccumQuantity { internal set; get; }

        public ulong OldContractId { internal set; get; }

        public Guid OrderClientId { internal set; get; }

        public decimal GetFilledPrice()
        {
            if (this.Trades.Count == 0)
            {
                return 0;
            }

            return this.Trades.Average(prop => prop.Price);
        }

        public decimal GetFilledVolume()
        {
            if (this.Trades.Count == 0)
            {
                return 0;
            }

            return this.Trades.Sum(prop => prop.Quantity);
        }

        public void OrderChanged()
        {
            OrderStatusChanged?.Invoke(this);
        }

        public bool Cancel() => Account?.CancelOrder(this) ?? false;

        public void Dispose()
        {
            OrderStatusChanged = null;
            ErrorEvent = null;
        }
    }
}
