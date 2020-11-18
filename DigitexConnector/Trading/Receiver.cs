using System;
using DigitexConnector.Orders;
using DigitexConnector.EngineAPI;
using System.Threading;
using DigitexWire;
using System.Collections.Generic;
using System.Linq;

namespace DigitexConnector.Trading
{
    internal class Receiver
    {
        private readonly ReaderWriterLockSlim _ordersLock;
        private readonly NotificationDictionary<Guid, OrderBase> _orders;

        internal Receiver(NotificationDictionary<Guid, OrderBase> orders, ReaderWriterLockSlim ordersLock)
        {
            _ordersLock = ordersLock;
            _orders = orders;
        }

        internal void OrderStatusHandle(OrderStatusData data)
        {
            Guid guid = data.OrigClientId;
            _ordersLock.EnterUpgradeableReadLock();
            try
            {
                if (!_orders.ContainsKey(guid))
                {
                    if (data.Status != OrderStatus.Rejected && data.Status != OrderStatus.Canceled)
                    {
                        _ordersLock.EnterWriteLock();
                        try
                        {
                            _orders.Add(guid, data.OrderType == OrderType.Limit ?
                                (OrderBase)new OrderLimit(data, data.Symbol) :
                                new OrderMarket(data, data.Symbol));
                            _orders[guid].TimeStamp = data.OrderTimestamp;
                        }
                        finally
                        { _ordersLock.ExitWriteLock(); }
                    }
                }
                else
                {
                    if (data.Status == OrderStatus.Rejected || data.Status == OrderStatus.Canceled)
                    {
                        _orders[guid].TimeStamp = data.OrderTimestamp;
                        _orders[guid].Status = data.Status;
                        _orders[guid].OrderChanged();

                        _ordersLock.EnterWriteLock();
                        try
                        {
                            _orders[guid].Dispose();
                            _orders.Remove(guid);
                        }
                        finally
                        { _ordersLock.ExitWriteLock(); }
                    }
                    else
                    {
                        _orders[guid].TimeStamp = data.OrderTimestamp;
                        _orders[guid].OpenTime = data.OpenTime;
                        _orders[guid].OrigClientId = data.OrigClientId;
                        _orders[guid].OrigQuantity = data.OrigQuantity;
                        _orders[guid].Status = data.Status;

                        _orders[guid].OrderChanged();

                    }
                }
            }
            finally
            { _ordersLock.ExitUpgradeableReadLock(); }
        }

        internal void OrderFilledHandle(OrderFilledData data)
        {
            Guid guid = data.OrigClientId;
            _ordersLock.EnterUpgradeableReadLock();
            try
            {
                if (_orders.ContainsKey(guid) && data.Status == OrderStatus.Partial)
                {
                    //_orders[data.ClientId].FilledQuantity += data.Quantity;
                    //Guid newClientId = data.NewClientId;
                    ////_orders[newClientId] = _orders[data.ClientId];
                    //_orders.Add(newClientId, _orders[data.ClientId]);
                    _orders[guid].PaidPrice = data.PaidPrice;
                    _orders[guid].Type = data.OrderType;
                    _orders[guid].Side = data.Side;
                    _orders[guid].Leverage = data.Leverage;
                    _orders[guid].Duration = data.Duration;
                    _orders[guid].Price = data.Price;
                    _orders[guid].Quantity = data.Quantity;
                    //OrderBase tmpOrder = _orders[data.ClientId];
                    //tmpOrder.Status = OrderStatus.Accepted;
                    //_orders[newClientId].ClientId = newClientId;
                    //_orders[newClientId].OrigClientId = data.OrigClientId;
                    _orders[guid].OrigQuantity = data.OrigQuantity;
                    _orders[guid].OpenTime = data.OpenTime;
                    foreach (var trade in data.RawTrades)
                    {
                        _orders[guid].Trades.Add(trade);
                    }
                    _orders[guid].Status = data.Status;

                    _orders[guid].OrderChanged();


                    //_ordersLock.EnterWriteLock();
                    //try
                    //{
                    //    _orders.Remove(data.ClientId);
                    //}
                    //finally
                    //{ _ordersLock.ExitWriteLock(); }
                }
                else if (_orders.ContainsKey(guid) && data.Status == OrderStatus.Filled)
                {
                    foreach (var trade in data.RawTrades)
                    {
                        _orders[guid].Trades.Add(trade);
                    }

                    _orders[guid].Status = data.Status;

                    _orders[guid].OrderChanged();

                    _ordersLock.EnterWriteLock();
                    try
                    {
                        _orders[guid].Dispose();
                        _orders.Remove(guid);
                    }
                    finally
                    { _ordersLock.ExitWriteLock(); }
                }
            }
            finally
            { _ordersLock.ExitUpgradeableReadLock(); }
        }

        internal void TraderStatusHandle(TraderStatusData data)
        {
            _ordersLock.EnterUpgradeableReadLock();
            try
            {
                foreach (OrderBase order in data.Orders)
                {
                    if (!_orders.ContainsKey(order.OrigClientId))
                    {
                        _ordersLock.EnterWriteLock();
                        try
                        { _orders.Add(order.OrigClientId, order); }
                        finally
                        { _ordersLock.ExitWriteLock(); }
                    }
                }
                foreach (Guid guid in _orders.Keys)
                {
                    if (_orders[guid].TargetSymbol.MarketId != data.MarketId)
                    { continue; }
                    if (data.Orders.FirstOrDefault(x => x.OrigClientId == guid) is null)
                    {
                        _orders[guid].Status = OrderStatus.StatusUndefined;
                        _orders[guid].OrderChanged();
                        _orders.Remove(guid); 
                    }
                }
            }
            finally
            { _ordersLock.ExitUpgradeableReadLock(); }
        }

        internal void UpdateOrders(List<OrderBase> orders)
        {
            _ordersLock.EnterUpgradeableReadLock();
            try
            {
                foreach (OrderBase order in orders)
                {
                    if (!_orders.ContainsKey(order.OrigClientId))
                    {
                        _ordersLock.EnterWriteLock();
                        try
                        { _orders.Add(order.OrigClientId , order); }
                        finally
                        { _ordersLock.ExitWriteLock(); }
                    }
                    else
                    {
                        //OrderBase oldOrder;
                        //if (!_orders.TryGetValue(order.OldClientId, out oldOrder))
                        //{ oldOrder = _orders[order.OrigClientId]; }
                        //Guid oldGuid = oldOrder.ClientId;
                        //_orders[order.OrigClientId].AccumQuantity = order.AccumQuantity;
                        //_orders[order.OrigClientId].ClientId = order.ClientId;
                        //_orders[order.OrigClientId].Duration = order.Duration;
                        //_orders[order.OrigClientId].FilledQuantity = order.FilledQuantity;
                        _orders[order.OrigClientId].Leverage = order.Leverage;
                        //_orders[order.OrigClientId].OldClientId = order.OldClientId;
                        //_orders[order.OrigClientId].OldContractId = order.OldContractId;
                        //_orders[order.OrigClientId].OpenTime = order.OpenTime;
                        //_orders[order.OrigClientId].OrderClientId = order.OrderClientId;
                        //_orders[order.OrigClientId].OrigQuantity = order.OrigQuantity;
                        //_orders[order.OrigClientId].PaidPrice = order.PaidPrice;
                        //_orders[order.OrigClientId].Price = order.Price;
                        //_orders[order.OrigClientId].Quantity = order.Quantity;
                        //_orders[order.OrigClientId].Side = order.Side;
                        //_orders[order.OrigClientId].Status = order.Status;
                        //_orders[order.OrigClientId].StopLossPrice = order.StopLossPrice;
                        //_orders[order.OrigClientId].StopLossType = order.StopLossType;
                        //_orders[order.OrigClientId].TimeStamp = order.TimeStamp;
                        //_orders[order.OrigClientId].Type = order.Type;
                        //_ordersLock.EnterWriteLock();
                        //try
                        //{
                        //    _orders.Add(order.ClientId, oldOrder);
                        //    _orders.Remove(oldGuid);
                        //}
                        //finally
                        //{ _ordersLock.ExitWriteLock(); }
                        //oldOrder.OrderChanged();
                    }
                }
            }
            finally
            { _ordersLock.ExitUpgradeableReadLock(); }
        }

        internal void OrderCanceledHandle(OrderCanceledData data)
        {
            _ordersLock.EnterUpgradeableReadLock();
            try
            {
                foreach (OrderBase order in data.Orders)
                {
                    Guid guid = order.OrigClientId;
                    if (_orders.ContainsKey(guid))
                    {
                        _orders[guid].Status = data.Status;
                        _orders[guid].OrderChanged();
                        _ordersLock.EnterWriteLock();
                        try
                        {
                            _orders[guid].Dispose();
                            _orders.Remove(guid); 
                        }
                        finally
                        { _ordersLock.ExitWriteLock(); }
                    }
                }
            }
            finally
            { _ordersLock.ExitUpgradeableReadLock(); }
        }

        internal void ErrorHandle(Guid guid, ErrorCodes errorCode)
        {
            _ordersLock.EnterUpgradeableReadLock();
            try
            {
                if (_orders.ContainsKey(guid))
                { _orders[guid].OnError(errorCode); }
            }
            finally
            { _ordersLock.ExitUpgradeableReadLock(); }
        }
    }
}
