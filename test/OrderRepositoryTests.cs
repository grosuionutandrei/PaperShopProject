using infrastructure;
using infrastructure.Models;
using infrastructure.QuerryModels;
using infrastructure.Repository.Orders;
using Microsoft.EntityFrameworkCore;

namespace test;

public class OrderRepositoryTests : IDisposable
{
    private readonly DataBaseContext _dbContext;
    private readonly OrderRepository _orderRepository;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DataBaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new DataBaseContext(options);
        _orderRepository = new OrderRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task GetOrdersByCustomerId_ShouldReturnOrders_WhenCustomerExistsWithOrders()
    {
        // Arrange
        var customer = new Customer { Id = 1, Name = "John Doe" };
        var orders = new List<Order>
        {
            new()
            {
                Id = 1,
                CustomerId = 1,
                OrderDate = DateTime.Now.AddDays(-10),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                Status = "Completed",
                TotalAmount = 100,
                Deleted = false
            },
            new()
            {
                Id = 2,
                CustomerId = 1,
                OrderDate = DateTime.Now.AddDays(-8),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                Status = "Pending",
                TotalAmount = 200,
                Deleted = false
            }
        };
        _dbContext.Customers.Add(customer);
        _dbContext.Orders.AddRange(orders);
        _dbContext.SaveChanges();

        // Act
        var result = await _orderRepository.GetOrdersByCustomerId(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, o => o.Status == "Completed");
        Assert.Contains(result, o => o.Status == "Pending");
    }

    [Fact]
    public async Task GetOrdersByCustomerId_ShouldReturnEmptyList_WhenCustomerExistsWithoutOrders()
    {
        // Arrange
        var customer = new Customer { Id = 2, Name = "Jane Doe" };
        _dbContext.Customers.Add(customer);
        _dbContext.SaveChanges();

        // Act
        var result = await _orderRepository.GetOrdersByCustomerId(2);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetOrdersByCustomerId_ShouldReturnEmptyList_WhenCustomerDoesNotExist()
    {
        // Arrange
        // No customers or orders added

        // Act
        var result = await _orderRepository.GetOrdersByCustomerId(99);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetOrdersByCustomerId_ShouldExcludeDeletedOrders()
    {
        // Arrange
        var customer = new Customer { Id = 3, Name = "Mark Smith" };
        var orders = new List<Order>
        {
            new()
            {
                Id = 3,
                CustomerId = 3,
                OrderDate = DateTime.Now.AddDays(-7),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                Status = "Completed",
                TotalAmount = 150,
                Deleted = false
            },
            new()
            {
                Id = 4,
                CustomerId = 3,
                OrderDate = DateTime.Now.AddDays(-6),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                Status = "Canceled",
                TotalAmount = 50,
                Deleted = true
            }
        };
        _dbContext.Customers.Add(customer);
        _dbContext.Orders.AddRange(orders);
        _dbContext.SaveChanges();

        // Act
        var result = await _orderRepository.GetOrdersByCustomerId(3);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.DoesNotContain(result, o => o.Status == "Canceled");
    }

    [Fact]
    public async Task GetOrdersByCustomerId_ShouldReturnOnlyActiveOrders()
    {
        // Arrange
        var customer = new Customer { Id = 4, Name = "Alice Johnson" };
        var orders = new List<Order>
        {
            new()
            {
                Id = 5,
                CustomerId = 4,
                OrderDate = DateTime.Now.AddDays(-10),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
                Status = "Shipped",
                TotalAmount = 300,
                Deleted = false
            },
            new()
            {
                Id = 6,
                CustomerId = 4,
                OrderDate = DateTime.Now.AddDays(-8),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
                Status = "Processing",
                TotalAmount = 400,
                Deleted = false
            },
            new()
            {
                Id = 7,
                CustomerId = 4,
                OrderDate = DateTime.Now.AddDays(-6),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                Status = "Canceled",
                TotalAmount = 500,
                Deleted = true
            }
        };
        _dbContext.Customers.Add(customer);
        _dbContext.Orders.AddRange(orders);
        _dbContext.SaveChanges();

        // Act
        var result = await _orderRepository.GetOrdersByCustomerId(4);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, o => o.Status == "Shipped");
        Assert.Contains(result, o => o.Status == "Processing");
        Assert.DoesNotContain(result, o => o.Status == "Canceled");
    }

    [Fact]
    public async Task GetOrdersByCustomerId_ShouldReturnEmptyList_WhenAllOrdersAreDeleted()
    {
        // Arrange
        var customer = new Customer { Id = 5, Name = "Bob Williams" };
        var orders = new List<Order>
        {
            new()
            {
                Id = 8,
                CustomerId = 5,
                OrderDate = DateTime.Now.AddDays(-12),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
                Status = "Returned",
                TotalAmount = 600,
                Deleted = true
            },
            new()
            {
                Id = 9,
                CustomerId = 5,
                OrderDate = DateTime.Now.AddDays(-10),
                DeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-4)),
                Status = "Refunded",
                TotalAmount = 700,
                Deleted = true
            }
        };
        _dbContext.Customers.Add(customer);
        _dbContext.Orders.AddRange(orders);
        _dbContext.SaveChanges();

        // Act
        var result = await _orderRepository.GetOrdersByCustomerId(5);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEntriesForOrder_ShouldReturnEntries_WhenOrderExistsWithEntries()
    {
        // Arrange
        var order = new Order { Id = 1, Status = "Pending" };
        var paper = new Paper
        {
            Id = 1,
            Name = "Paper A",
            Price = 10.00,
            Properties = new List<Property>
            {
                new() { Id = 1, PropertyName = "Glossy" },
                new() { Id = 2, PropertyName = "Matte" }
            }
        };
        var orderEntries = new List<OrderEntry>
        {
            new() { Id = 1, OrderId = 1, Quantity = 5, Product = paper },
            new() { Id = 2, OrderId = 1, Quantity = 10, Product = paper }
        };
        _dbContext.Orders.Add(order);
        _dbContext.Papers.Add(paper);
        _dbContext.OrderEntries.AddRange(orderEntries);
        _dbContext.SaveChanges();

        // Act
        var result = await _orderRepository.GetEntriesForOrder(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, e => Assert.Equal("Paper A", e.PaperName));
        Assert.All(result, e => Assert.Equal(10.00, e.Price));
        Assert.All(result, e => Assert.Equal(2, e.PaperProperties.Count()));
    }

    [Fact]
    public async Task ModifyOrderStatus_ShouldReturnTrue_WhenOrderExists()
    {
        // Arrange
        var order = new Order { Id = 1, Status = "Pending" };
        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();

        // Act
        var result = await _orderRepository.ModifyOrderStatus(1, "Completed");

        // Assert
        Assert.True(result);
        var updatedOrder = await _dbContext.Orders.FindAsync(1);
        Assert.Equal("Completed", updatedOrder.Status);
    }

    [Fact]
    public void GetProductsPrices_ShouldReturnCorrectPrices_WhenProductsExist()
    {
        // Arrange
        var papers = new List<Paper>
        {
            new() { Id = 1, Price = 10.0, Name = "test" },
            new() { Id = 2, Price = 20.0, Name = "test2" }
        };
        _dbContext.Papers.AddRange(papers);
        _dbContext.SaveChanges();

        var orderEntries = new List<OrderEntryPlaced>
        {
            new() { ProductId = 1 },
            new() { ProductId = 2 }
        };

        // Act
        var result = _orderRepository.GetProductsPrices(orderEntries);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(10.0, result[1]);
        Assert.Equal(20.0, result[2]);
    }

    [Fact]
    public async Task GetCustomerOrderHistory_ShouldReturnOrders_WhenCustomerExists()
    {
        // Arrange
        var customer = new Customer { Id = 1, Name = "John Doe" };
        var orders = new List<Order>
        {
            new() { Id = 1, CustomerId = 1, Status = "Completed" },
            new() { Id = 2, CustomerId = 1, Status = "Pending" }
        };
        _dbContext.Customers.Add(customer);
        _dbContext.Orders.AddRange(orders);
        _dbContext.SaveChanges();

        // Act
        var result = await _orderRepository.GetCustomerOrderHistory(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, o => o.Status == "Completed");
        Assert.Contains(result, o => o.Status == "Pending");
    }
}