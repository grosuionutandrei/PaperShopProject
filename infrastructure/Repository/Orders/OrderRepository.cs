
using infrastructure.Models;
using infrastructure.QuerryModels;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository.Orders;

public class OrderRepository : IOrderRepository
{

    private readonly DataBaseContext _dataBaseContext;

    public OrderRepository(DataBaseContext db)
    {
        _dataBaseContext = db;
    }



    public async Task<IEnumerable<OrderMain>> GetOrdersByCustomerId(int customerId)
    {
        var isCustomer = await _dataBaseContext.Customers.Where(e => e.Id == customerId).FirstOrDefaultAsync();
        if (isCustomer != null && isCustomer!.Id != customerId)
        {
            return new List<OrderMain>();
        }

        var customerOrders = await _dataBaseContext.Orders.Where(e => e.CustomerId == customerId)
            .Where(e => e.Deleted != true)
            .Select(e => new OrderMain
            {
                Id = e.Id,
                OrderDate = e.OrderDate,
                DeliveryDate = e.DeliveryDate,
                Status = e.Status,
                TotalAmount = e.TotalAmount

            })
            .ToListAsync();
        return customerOrders;
    }


    public async Task<IEnumerable<OrderEntryQto>> GetEntriesForOrder(int orderId)
    {

        var order = await _dataBaseContext.Orders.Where(e => e.Id == orderId).FirstOrDefaultAsync();
        if (order == null)
        {
            return new List<OrderEntryQto>();
        }

        var orderEntries = await _dataBaseContext.OrderEntries.Where(e => e.OrderId == orderId)
            .Select(e => new OrderEntryQto
            {
                Id = e.Id,
                OrderQuantity = e.Quantity,
                PaperName = e.Product!.Name,
                Price = e.Product.Price,
                PaperProperties = e.Product.Properties.Select(p => new PaperProperties
                {
                    PropId = p.Id,
                    PropName = p.PropertyName
                })

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

    public  Dictionary<int, double> GetProductsPrices(List<OrderEntryPlaced> orderEntries)
    {
        Dictionary<int, double> paperPrices = new Dictionary<int, double>();
        var productsIds = orderEntries.Select(p => p.ProductId).ToList();
        var productPrices = _dataBaseContext.Papers.Where(p => productsIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Price }).ToList();
        foreach (var product in productPrices)
        {
            paperPrices[product.Id] = product.Price;
        }

        return paperPrices;
    }

    public async Task<IEnumerable<OrderMain>> GetCustomerOrderHistory(int customerId)
    {
        var customer = await _dataBaseContext.Customers.Where(e=>e.Id==customerId).FirstOrDefaultAsync();
        if (customer==null)
        {
            return new List<OrderMain>();
        }

        var orders = await _dataBaseContext.Orders.Where(e => e.CustomerId == customerId)
            .Select(e => new OrderMain
            {
                Id = e.Id,
                OrderDate = e.OrderDate,
                DeliveryDate = e.DeliveryDate,
                Status = e.Status,
                TotalAmount = e.TotalAmount
            })
            .ToListAsync();
        return orders;
    }


    public async Task<OrderMain> PlaceOrder(int customerId, OrderPlaced orderPlaced)
    {
        using var transaction = await _dataBaseContext.Database.BeginTransactionAsync();
        try
        {
            var customer = await _dataBaseContext.Customers.Where(e => e.Id == customerId).FirstOrDefaultAsync();
            if (customer == null)
            {
                return new OrderMain();
            }

            var newOrder = new Order
            {
                OrderDate = orderPlaced.OrderDate,
                DeliveryDate = orderPlaced.DeliveryDate,
                Status = orderPlaced.Status,
                TotalAmount = orderPlaced.TotalAmount,
                CustomerId = customerId,
                Deleted = orderPlaced.Deleted,
            };

            await _dataBaseContext.Orders.AddAsync(newOrder);
            await _dataBaseContext.SaveChangesAsync();

            var orderId = newOrder.Id;

            var orderEntries = orderPlaced.OrderProducts!.Select(op => new OrderEntry
            {
                OrderId = orderId,
                ProductId = op.ProductId,
                Quantity = op.Quantity
            }).ToList();

            await _dataBaseContext.OrderEntries.AddRangeAsync(orderEntries);
            await _dataBaseContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return new OrderMain
            {
                Id = orderId,
                OrderDate = newOrder.OrderDate,
                DeliveryDate = newOrder.DeliveryDate,
                Status = newOrder.Status,
                TotalAmount = newOrder.TotalAmount
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}