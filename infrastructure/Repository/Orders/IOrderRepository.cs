using infrastructure.QuerryModels;

namespace infrastructure.Repository.Orders;

public interface IOrderRepository
{
    public Task<IEnumerable<OrderMain>> GetOrdersByCustomerId(int customerId);
    Task<IEnumerable<OrderEntryQto>> GetEntriesForOrder(int orderId);

    Task<bool> ModifyOrderStatus(int orderId, string status);
    Task<OrderMain> PlaceOrder(int customerId,OrderPlaced orderPlaced);
    Dictionary<int, double> GetProductsPrices(List<OrderEntryPlaced> orderEntries);
    Task<IEnumerable<OrderMain>> GetCustomerOrderHistory(int customerId);
    Task<IEnumerable<CustomerMain>> GetCustomers();
    Task<bool> UpdateOrderStatus(string? statusStatus,int orderId);
}