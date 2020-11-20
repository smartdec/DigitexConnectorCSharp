using DigitexConnector;
using DigitexConnector.EngineAPI;
using DigitexConnector.Orders;
using DigitexConnector.Trading;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Example
{
    class IntervalAlgorithm : TradingAlgorithm
    {
        private int Interval = 10;
        private string SymbolName = "BTC/USD1";
        private Symbol Smbl;
        private OrderBook OBook;

        private Timer TradingTimer;

        public IntervalAlgorithm(string host, string token, bool wss)
            : base(host, token, wss)
        { }

        public override void Prepare()
        {
            Smbl = SymbolsContainer.GetSymbol(SymbolName);
            SpotPriceUpdated += SpotPriceHandler;
            OrderBookUpdated += OrderBookHandler;
            OBook = TrackSymbol(Smbl);
            TradingTimer = new Timer((object obj) => PlaceOrder(), null, Interval * 1000, Interval * 1000);
        }

        private void SpotPriceHandler(OrderBook oBook)
        {
            if (oBook.Equals(OBook))
            {
                // Do something for handle spot price changing.
            }
        }

        private void OrderBookHandler(OrderBook oBook)
        {
            if (oBook.Equals(OBook))
            {
                // Do something for handle market depth changing.
            }
        }

        private void PlaceOrder()
        {
            bool positivePosition = Account.GetTraderInfo(Smbl).PositionType == DigitexWire.OrderPosition.Long;
            decimal price;
            DigitexWire.OrderSide side;

            if (positivePosition)
            {
                price = OBook.GetBestAskPrice() ?? OBook.AdjustedSpotPrice;
                side = DigitexWire.OrderSide.Sell;
            }
            else
            {
                price = OBook.GetBestBidPrice() ?? OBook.AdjustedSpotPrice;
                side = DigitexWire.OrderSide.Buy;
            }

            OrderBase order = PlaceOrderLimit(Smbl, side, 1, price, OrderStatusHandler, ErrorHandler);

            Console.WriteLine($"Place {order?.OrigClientId} - {(order is null ? "fail" : "success")}");
        }

        private void OrderStatusHandler(OrderBase order)
        {
            // Do something for handle order status changing.

            Console.WriteLine($"Order {order.OrigClientId} is {order.Status}");

            // Use OrigClientId for comparison of orders.
        }

        private void ErrorHandler(OrderBase order, ErrorCodes code)
        {
            // Do something for handle order errors.

            Console.WriteLine($"Error {code}, order {order.OrigClientId}");
        }

        public override List<ModuleState> GetParams()
        {
            return new List<ModuleState>()
            {
                new ModuleState("interval", "10")
            };
        }

        public override void SetAlgoArguments(Dictionary<string, string> algoArguments)
        {
            if (algoArguments.ContainsKey("interval")) { Interval = int.Parse(algoArguments["interval"]); }
        }

        public override void Stop()
        {
            TradingTimer.Dispose();
        }

        public override void OnDispose()
        {
            TradingTimer?.Dispose();
        }
    }
}
