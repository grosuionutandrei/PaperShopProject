using infrastructure.QuerryModels;

namespace service.Orders;

public interface IOrderService
{
    Task<IEnumerable<OrderMain>> GetOrdersByCustomerId(int customerId);
    Task<IEnumerable<OrderEntry>> GetEntriesForOrder(int orderId);
    Task<OrderMain> GetOrderByOrderId(int orderId);
    Task<OrderMain> GetAllOrdersPaginated(int pageNumber, int itemsPerPage);

    Task<bool> ModifyOrderStatus(int orderId, string status);
}