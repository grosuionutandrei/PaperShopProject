using infrastructure.QuerryModels;
using infrastructure.Repository;
using infrastructure.Repository.Orders;


namespace service.Orders;

public class OrderService:IOrderService
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

    public Task<IEnumerable<OrderEntry>> GetEntriesForOrder(int orderId)
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
        return _repository.ModifyOrderStatus(orderId,status);
    }
}