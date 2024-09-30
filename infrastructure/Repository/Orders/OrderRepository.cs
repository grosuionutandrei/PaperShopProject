using System.Collections;
using infrastructure.QuerryModels;
using Microsoft.EntityFrameworkCore;
using OrderEntry = infrastructure.QuerryModels.OrderEntry;

namespace infrastructure.Repository.Orders;

public class OrderRepository:IOrderRepository
{
    
    private readonly DataBaseContext _dataBaseContext;

    public OrderRepository(DataBaseContext db)
    {
        _dataBaseContext = db;
    }
    
    
    
    public  async Task<IEnumerable<OrderMain>> GetOrdersByCustomerId(int customerId)
    {
        var isCustomer = await _dataBaseContext.Customers.Where(e => e.Id == customerId).FirstOrDefaultAsync();
        if (isCustomer!=null && isCustomer!.Id != customerId)
        {
            return new List<OrderMain>();
        }

        var customerOrders =await _dataBaseContext.Orders.Where(e => e.CustomerId == customerId).Where(e=>e.Deleted!=true)
            .Select(e=>new OrderMain
            {
                Id=e.Id,
                OrderDate = e.OrderDate,
                DeliveryDate = e.DeliveryDate,
                Status = e.Status,
                TotalAmount = e.TotalAmount
                
            })
            .ToListAsync();
        return customerOrders;
    }


    public async Task<IEnumerable<OrderEntry>> GetEntriesForOrder(int orderId)
    {
         
        var order = await _dataBaseContext.Orders.Where(e => e.Id == orderId).FirstOrDefaultAsync();
        if (order == null)
        {
            return new List<OrderEntry>();
        }

        var orderEntries = await _dataBaseContext.OrderEntries.Where(e => e.OrderId == orderId)
            .Select(e => new OrderEntry
            {
             Id=e.Id,
             OrderQuantity = e.Quantity,
             PaperName = e.Product!.Name,
             Price = e.Product.Price,
             PaperProperties = e.Product.Properties.Select(p=>new PaperProperties
             {
               PropId  = p.Id,
               PropName = p.PropertyName
             } )
             
            }).ToListAsync();
        return orderEntries;

    }

    public async Task<bool> ModifyOrderStatus(int orderId, string status)
    {
        var orderToModify = await _dataBaseContext.Orders.Where(e => e.Id == orderId).FirstOrDefaultAsync();
        if (orderToModify == null)
        {
            return false;
        }

        orderToModify.Status = status;
         _dataBaseContext.Orders.Update(orderToModify);
         await _dataBaseContext.SaveChangesAsync();
        return true;
    }
}