﻿using infrastructure.QuerryModels;
using infrastructure.Repository.Orders;
using utilities.OrderStatus;

namespace service.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<OrderMain>> GetOrdersByCustomerId(int customerId)
    {
        return _repository.GetOrdersByCustomerId(customerId);
    }

    public Task<IEnumerable<OrderEntryQto>> GetEntriesForOrder(int orderId)
    {
        return _repository.GetEntriesForOrder(orderId);
    }

    public Task<OrderMain> GetOrderByOrderId(int orderId)
    {
        throw new NotImplementedException();
    }

    public Task<OrderMain> GetAllOrdersPaginated(int pageNumber, int itemsPerPage)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ModifyOrderStatus(int orderId, string status)
    {
        return _repository.ModifyOrderStatus(orderId, status);
    }

    public Task<OrderMain> PlaceOrder(int customerId, List<OrderEntryPlaced> orderEntries)
    {
        var currentDate = DateTime.UtcNow;
        var standardDeliveryDate = DateOnly.FromDateTime(currentDate.AddDays(3));
        var Status = OrderStatusMessage.GetMessage(OrderStatus.Pending);
        var ProductsPrices = _repository.GetProductsPrices(orderEntries);

        var totalAmount = orderEntries.Sum(oe =>
        {
            ProductsPrices.TryGetValue(oe.ProductId, out var price);
            return oe.Quantity * price;
        });

        var orderPlaced = new OrderPlaced
        {
            OrderDate = currentDate,
            DeliveryDate = standardDeliveryDate,
            Status = Status,
            OrderProducts = orderEntries,
            TotalAmount = totalAmount
        };

        return _repository.PlaceOrder(customerId, orderPlaced);
    }

    public Task<IEnumerable<OrderMain>> GetCustomerOrderHistory(int customerId)
    {
        return _repository.GetCustomerOrderHistory(customerId);
    }

    public Task<IEnumerable<CustomerMain>> GetCustomers()
    {
        return _repository.GetCustomers();
    }

    public bool ValidateStatus(string orderStatus)
    {
        return OrderStatusMessage.IsStatusValid(orderStatus);
    }

    public Task<bool> UpdateOrderStatus(string? statusStatus, int orderId)
    {
        return _repository.UpdateOrderStatus(statusStatus, orderId);
    }
}