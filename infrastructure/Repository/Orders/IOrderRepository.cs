using infrastructure.QuerryModels;

namespace infrastructure.Repository.Orders;

public interface IOrderRepository
{
    public Task<IEnumerable<OrderMain>> GetOrdersByCustomerId(int customerId);
    Task<IEnumerable<OrderEntry>> GetEntriesForOrder(int orderId);

    Task<bool> ModifyOrderStatus(int orderId, string status);
}