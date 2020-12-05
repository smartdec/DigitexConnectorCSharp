**Manual for algorithm implementation**


>  There is the example of Interval algorithm.


Every algo is inherited from the class **TradingAlgorithm**.

    What you can do:
        1. subscribe for SpotPriceUpdated, OrderBookUpdated, TraderInfoChanged, 
           NewTrades, Connected, Reconnected and Disconnected events.
        2. take information about your account and current order book.

**Account**

        This object has the following properties to make implementation easier:
            List<TraderInfo> TraderStatistic - General indicators of a trader from the exchange.
            NotificationDictionary<Guid, OrderBase> Orders - colection of all active orders.
            List<OrderTrailingStop> TrailingOrders - collection of all active trailing stop orders.
            
            
**OrderBook**

        This object has the following properties and methods to make implementation easier:

            decimal GetBestAskPrice() // Return min ask price if ask.Count is not 0 otherwise null;
            decimal GetBestBidPrice() // Return max bid price if ask.Count is not 0 otherwise null;
            decimal LastTradePrice;
            decimal LastTradeQuantity;
            decimal SpotPrice;
            decimal AdjustedSpotPrice - rounded spot price;
            Dictionary<decimal, decimal> Asks - collection of asks;
            Dictionary<decimal, decimal> Bids - collection of bids;
            DateTime LastFullUpdateTimestamp;

            
        To create an object of this class, use the method 
        Account.TrackSymbol(
            Symbol symbol, 
            Action<OrderBook> spotPriceUpdatedHandler = null, 
            Action<OrderBook> orderBookUpdatedHandler = null, 
            Action<OrderBook> orderBookConnectedHandler = null, 
            Action<OrderBook> orderBookDisconnectedHandler = null,
            Action<OrderBook> orderBookReconnectedHandler = null
        )

        Pass the appropriate delegates to the method for processing orderbook events.
        To make a symbol, use SymbolsContainer.GetSymbol(SymbolName).
        
            
**SpotPriceUpdated**
        Receive all updates of spot price.
        To hanlde this event your method must be like this:
        
        public void SpotPriceUpdatedHandler(OrderBook orderBook)
            
**OrderBookUpdated**
        Receive reference to order book when updates are received.
        To hanlde this event your method must be like this:
        
        public void OnOrderBookUpdatedHandler(OrderBook orderBook)
        
**TraderInfoUpdated**
        Receive updates of changes in Account.
        To hanlde this event your method must be like this:
        
        public void OnTraderInfoHandler(TraderInfo traderInfo);
        
You also need to override the following methods:

        public abstract void Prepare(); - this method adds events subscriptions and called before trading.
        public virtual void SetAlgoArguments(Dictionary<string, string> algoArguments); - (not necessary) if your 
        algorithm has parameters you can initialize them with this methods.
        public virtual List<ModuleState> GetParams(); - (not necessary) returns parameters of algorithm.
        public abstract void OnDispose(); - this method called before dispose.

Example of these methods implementation:

        Prepare:
                Here you can initialize timer or run CancelAllOrders or another.
                
        SetAlgoArguments:
                if (algoArguments.ContainsKey("tradingInterval"))
                { tradingInterval = int.Parse(algoArguments["tradingInterval"]); }
                if (algoArguments.ContainsKey("tradingQuantity"))
                { tradingQuantity = int.Parse(algoArguments["tradingQuantity"]); }
                         
        GetParams:
                Here just return default values of parameters that you need.

**How to send messages in algorithm:**

In your algo you can call following methods:

        OrderLimit PlaceOrderLimit(
            OrderSide side, 
            decimal quantity, 
            decimal price, 
            Action<OrderBase> orderStatusHandler = null, 
            Action<OrderBase, ErrorCodes> errorHandler = null
        );
        Pass the appropriate delegates to the method for processing order events.
        
        OrderMarket PlaceOrderMarket(
            OrderSide side, 
            decimal quantity, 
            Action<OrderBase> orderStatusHandler = null, 
            Action<OrderBase, ErrorCodes> errorHandler = null
        );
        Pass the appropriate delegates to the method for processing order events.
        
        OrderTrailingStop PlaceOrderTrailingStop(
            OrderSide side, 
            decimal quantity, 
            int lagTicks,
            Action<OrderBase> orderStatusHandler = null, 
            Action<OrderBase, ErrorCodes> errorHandler = null
        );
        Pass the appropriate delegates to the method for processing order events.
            
        The methods listed above return an OrderBase object if successful, otherwise null.
            
        Unknown parameters:
            decimal price - must be greater than 0 and multiple of appropriate Symbol.PriceStep;
            decimal quantity - must be greater than 0 and multiple of appropriate Symbol.QuantityStep;
            OrderSide side - available values are Buy or Sell;
            int lagTicks - number of intervals for which is behind and ahead of the trailing stop;
            Action<OrderBase> orderStatusHandler - when order status is changed and received from the exchange this handler is called;
            Action<OrderBase, ErrorCodes> errorHandler - if you receive some errors from the exchange this handler is called;
            
        Example:
            OrderBase order = PlaceOrderLimit(
                DigitexWire.OrderSide.Sell, 
                quantity, 
                price, 
                OrderStatusHandler, 
                ErrorHandler
            );
            
            public void OrderStatusHandler(OrderBase order)
            {
                Console.WriteLine(order.Status);
            }

            private void ErrorHandler(OrderBase order, DigitexConnector.EngineAPI.ErrorCodes errorCode)
            {
                Console.WriteLine(errorCode);
            }

How to launch you algorithm from your app:    
        
        DigitexConnector.Configuration.Server = Servers.testnet;
        DigitexConnector.Configuration.Token = "<your_API_token>";

        Algorithm = new YourAlgorithmClass();  // must be legacy from abstract class DigitexConnector.Trading.TradingAlgorithm or DigitexConnector.Trading.TradingAdapter
        Algorithm.AccountConnected += AccountConnectedHandler;  // Your implementation of connect to trade channel.
        Algorithm.AccountReconnected += AccountReconnectedHandler;  // Your implementation of reconnect to trade channel.
        Algorithm.AccountDisconnected += AccountDisconnectedHandler;  // Your implementation of disconnect from trade channel.
        Algorithm.TraderInfoUpdated += TraderInfoUpdatedHandler;  // Your implementation of TraderInfo updates.
        Algorithm.SetAlgoArguments(algoArguments);
        Algorithm.Prepare();
        Algorithm.Connect();  // After this you can send messages to exchange and receive trading and data information.

**How to work with static class SymbolsContainer**

        For load symbols, copy file Symbols.json from this repository to your project's target directory. 
        If you need for load symbols from another file, use SymbolsContainer.UpdateSymbols(string path), 
        where path - path to your file in the appropriate format. 
        For get symbols use SymbolsContainer.GetSymbol(string name) or SymbolsContainer.GetSymbol(int marketId).
