using System;
using System.Collections.Generic;
using DigitexConnector.Orders;
using DigitexConnector.Extentions;
using DigitexWire;

namespace DigitexConnector.EngineAPI
{
    public class LeverageData: CommonFields
    {
        public decimal LastTradePrice { private set; get; }
        public decimal LastTradeQuantity { private set; get; }
        public DateTime LastTradeTimestamp { private set; get; }
        public uint Leverage { private set; get; }
        public List<OrderBase> Orders { private set; get; }
        public decimal PositionBankruptcyVolume { private set; get; }
        public decimal PositionContracts { private set; get; }
        public decimal PositionLiquidationVolume { private set; get; }
        public OrderPosition PositionType { private set; get; }
        public decimal PositionVolume { private set; get; }
        public List<Trade> Trades { private set; get; }

        public LeverageData(Message message)
        {
            if (message.KontentCase != Message.KontentOneofCase.LeverageMsg)
            {
                throw new ArgumentException($"Error: Message containis KontentCase {message.KontentCase} " +
                  $" instead of LeverageMsg.");
            }
            Symbol symbol = SymbolsContainer.GetSymbol(message.MarketId);
            LeverageMessage leverageMessage = message.LeverageMsg;
            DWConverter.FromProtoDecimal(leverageMessage.AccumQuantity);
            BuyOrderMargin = DWConverter.FromProtoDecimal(leverageMessage.BuyOrderMargin);
            BuyOrderQuantity = DWConverter.FromProtoDecimal(leverageMessage.BuyOrderQuantity);
            LastTradePrice = DWConverter.FromProtoDecimal(leverageMessage.LastTradePrice);
            LastTradeQuantity = DWConverter.FromProtoDecimal(leverageMessage.LastTradeQuantity);
            LastTradeTimestamp = DWConverter.FromLongDateTime(leverageMessage.LastTradeTimestamp);
            Leverage = leverageMessage.Leverage;
            OrderMargin = DWConverter.FromProtoDecimal(leverageMessage.OrderMargin);

            Orders = new List<OrderBase>();
            foreach(OrderMessage orderMessage in leverageMessage.Orders)
            {
                OrderBase order = new OrderLimit(orderMessage, symbol);
                Orders.Add(order);
            }

            Pnl = DWConverter.FromProtoDecimal(leverageMessage.Pnl);
            PositionBankruptcyVolume = DWConverter.FromProtoDecimal(leverageMessage.PositionBankruptcyVolume);
            PositionContracts = DWConverter.FromProtoDecimal(leverageMessage.PositionContracts);
            PositionLiquidationVolume = DWConverter.FromProtoDecimal(leverageMessage.PositionLiquidationVolume);
            PositionMargin = DWConverter.FromProtoDecimal(leverageMessage.PositionMargin);
            PositionType = leverageMessage.PositionType;
            PositionVolume = DWConverter.FromProtoDecimal(leverageMessage.PositionVolume);
            SellOrderMargin = DWConverter.FromProtoDecimal(leverageMessage.SellOrderMargin);
            SellOrderQuantity = DWConverter.FromProtoDecimal(leverageMessage.SellOrderQuantity);
            TraderBalance = DWConverter.FromProtoDecimal(leverageMessage.TraderBalance);

            Trades = new List<Trade>();
            foreach(TradeMessage tradeMessage in leverageMessage.Trades)
            {
                Trade trade = new Trade(tradeMessage, symbol);
                Trades.Add(trade);
            }

            Upnl = DWConverter.FromProtoDecimal(leverageMessage.Upnl);
        }
    }
}
