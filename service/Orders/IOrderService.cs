using infrastructure.QuerryModels;

namespace service.Orders;

public interface IOrderService
{
    Task<IEnumerable<OrderMain>> GetOrdersByCustomerId(int customerId);
    Task<IEnumerable<OrderEntryQto>> GetEntriesForOrder(int orderId);
    Task<OrderMain> GetOrderByOrderId(int orderId);
    Task<OrderMain> GetAllOrdersPaginated(int pageNumber, int itemsPerPage);

    Task<bool> ModifyOrderStatus(int orderId, string status);
    Task<OrderMain> PlaceOrder(int customerId, List<OrderEntryPlaced> orderEntries);
    Task<IEnumerable<OrderMain>> GetCustomerOrderHistory(int customerId);
    Task<IEnumerable<CustomerMain>> GetCustomers();

    bool ValidateStatus(string orderStatus);
    Task<bool> UpdateOrderStatus(string? statusStatus, int orderId);
}