using System;
using System.Collections.Generic;
using DigitexConnector.EngineAPI;
using DigitexWire;
using DigitexConnector.Orders;
using System.Threading;
using DigitexConnector.Interfaces;

namespace DigitexConnector.Trading
{
    internal class Sender
    {
        private readonly ReaderWriterLockSlim _ordersLock;
        private readonly ReaderWriterLockSlim _trailingsLock;
        private readonly NotificationDictionary<Guid, OrderBase> _orders;
        private readonly List<OrderTrailingStop> _trailingOrders;
        private readonly IConnection _connection;

        internal Sender(NotificationDictionary<Guid, OrderBase> orders, List<OrderTrailingStop> trailingOrders, 
            IConnection connection, ReaderWriterLockSlim ordersLock, ReaderWriterLockSlim trailingsLock)
        {
            _ordersLock = ordersLock;
            _trailingsLock = trailingsLock;
            _orders = orders;
            _trailingOrders = trailingOrders;
            _connection = connection;
        }

        internal OrderLimit PlaceOrderLimit(OrderLimit order, Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null)
        {
            if (!_connection.IsControlConnected())
            { return null; }
            order.OrderStatusChanged += orderStatusHandler;
            order.ErrorEvent += errorHandler;
            _ordersLock.EnterWriteLock();
            try
            { _orders.Add(order.ClientId, order); }
            finally
            { _ordersLock.ExitWriteLock(); }
            bool result = _connection.PlaceOrder(order);
            if (!result)
            {
                _ordersLock.EnterWriteLock();
                try
                {
                    order.Dispose();
                    _orders.Remove(order.ClientId); 
                }
                finally
                { _ordersLock.ExitWriteLock(); }
                order = null;
            }
            return order;
        }

        internal OrderMarket PlaceOrderMarket(OrderMarket order, Action<OrderBase> orderStatusHandler = null, Action<OrderBase, ErrorCodes> errorHandler = null)
        {
            if (!_connection.IsControlConnected())
            { return null; }
            order.OrderStatusChanged += orderStatusHandler;
            order.ErrorEvent += errorHandler;
            _ordersLock.EnterWriteLock();
            try
            { _orders.Add(order.ClientId, order); }
            finally
            { _ordersLock.ExitWriteLock(); }
            bool result = _connection.PlaceOrder(order);
            if (!result)
            {
                _ordersLock.EnterWriteLock();
                try
                {
                    order.Dispose();
                    _orders.Remove(order.ClientId);
                }
                finally
                { _ordersLock.ExitWriteLock(); }
                order = null;
            }
            return order;
        }
        
        internal OrderTrailingStop PlaceOrderTrailingStop(OrderTrailingStop order, Action<OrderBase> orderStatusHandler = null,
                Action<OrderBase, ErrorCodes> errorHandler = null)
        {
            if (!_connection.IsControlConnected())
            { return null; }
            order.OrderStatusChanged += orderStatusHandler;
            order.ErrorEvent += errorHandler;
            _trailingsLock.EnterWriteLock();
            try
            { _trailingOrders.Add(order); }
            finally
            { _trailingsLock.ExitWriteLock(); }
            return order;
        }

        internal bool CancelOrder(OrderBase order)
        {
            if (!_connection.IsControlConnected())
            { return false; }
            bool result = false;
            _trailingsLock.EnterUpgradeableReadLock();
            try
            {
                if (order is OrderTrailingStop && _trailingOrders.Contains((OrderTrailingStop)order))
                {
                    _trailingsLock.EnterWriteLock();
                    try
                    {
                        order.Dispose();
                        _trailingOrders.Remove((OrderTrailingStop)order);
                        result = true;
                    }
                    finally
                    { _trailingsLock.ExitWriteLock(); }
                }
            }
            finally
            { _trailingsLock.ExitUpgradeableReadLock(); }
            if (result)
            { return true; }
            result = true;
            _ordersLock.EnterReadLock();
            try
            {
                if (!_orders.ContainsKey(order.ClientId))
                { result = false; }
            }
            finally
            { _ordersLock.ExitReadLock(); }
            if (!result)
            { return false; }
            Guid guid = Guid.NewGuid();
            result = _connection.CancelOrder(guid, order.ClientId, (uint)order.TargetSymbol.MarketId);
            return result;
        }

        internal bool CancelAllOrders(Symbol symbol)
        {
            if (!_connection.IsControlConnected())
            { return false; }
            Guid guid = Guid.NewGuid();
            bool result = _connection.CancelAllOrders(guid, (uint)symbol.MarketId);
            return result;
        }

        public bool UpdateTraderStatus()
        {
            if (!_connection.IsControlConnected())
            { return false; }
            Guid guid = Guid.NewGuid();
            return _connection.GetTraderStatus(guid);
        }

        internal void CheckTrailingStopOrders(OrderBook orderBook)
        {
            List<OrderTrailingStop> placedOrders = new List<OrderTrailingStop>();
            List<OrderTrailingStop> failedOrders = new List<OrderTrailingStop>();
            _trailingsLock.EnterReadLock();
            try
            {
                foreach (OrderTrailingStop order in _trailingOrders)
                {
                    if (!orderBook.TrackedSymbol.Equals(order.TargetSymbol))
                    { continue; }
                    bool result = true;
                    if (order.Side == OrderSide.Buy)
                    {
                        if (order.StrikePrice - orderBook.AdjustedSpotPrice > 5 * order.LagTicks)
                        { order.SetStrikePrice(orderBook.AdjustedSpotPrice); }
                        else if (order.StrikePrice - orderBook.AdjustedSpotPrice <= 0)
                        {
                            _ordersLock.EnterWriteLock();
                            try
                            { _orders.Add(order.ClientId, order); }
                            finally
                            { _ordersLock.ExitWriteLock(); }
                            placedOrders.Add(order);
                            result = _connection.PlaceOrder(order);
                        }
                    }
                    else
                    {
                        if (orderBook.AdjustedSpotPrice - order.StrikePrice > 5 * order.LagTicks)
                        { order.SetStrikePrice(orderBook.AdjustedSpotPrice); }
                        else if (orderBook.AdjustedSpotPrice - order.StrikePrice <= 0)
                        {
                            _ordersLock.EnterWriteLock();
                            try
                            { _orders.Add(order.ClientId, order); }
                            finally
                            { _ordersLock.ExitWriteLock(); }
                            placedOrders.Add(order);
                            result = _connection.PlaceOrder(order);
                        }
                    }
                    if (!result)
                    {
                        _ordersLock.EnterWriteLock();
                        try
                        {
                            order.Dispose();
                            _orders.Remove(order.ClientId); 
                        }
                        finally
                        { _ordersLock.ExitWriteLock(); }
                        placedOrders.Remove(order);
                        failedOrders.Add(order);
                    }
                }
            }
            finally
            { _trailingsLock.ExitReadLock(); }
            _trailingsLock.EnterWriteLock();
            try
            {
                foreach (OrderTrailingStop order in placedOrders)
                {
                    order.Dispose();
                    _trailingOrders.Remove(order); 
                }
                foreach (OrderTrailingStop order in failedOrders)
                {
                    order.Status = OrderStatus.Rejected;
                    order.Dispose();
                    _trailingOrders.Remove(order);
                }
            }
            finally
            { _trailingsLock.ExitWriteLock(); }
        }

    }
}
