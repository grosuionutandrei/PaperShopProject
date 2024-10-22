using api.Controllers;
using api.TransferModels;
using infrastructure.QuerryModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using service.Orders;

public class OrderControllerTests
{
    private readonly OrderController _controller;
    private readonly Mock<ILogger<OrderController>> _mockLogger;
    private readonly Mock<IOrderService> _mockOrderService;

    public OrderControllerTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _mockLogger = new Mock<ILogger<OrderController>>();
        _controller = new OrderController(_mockOrderService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetOrderByCustomerId_ShouldReturnOk_WhenOrdersExist()
    {
        // Arrange
        var customerId = 1;
        var orders = new List<OrderMain> { new() { Id = 1, Status = "Completed" } };
        _mockOrderService.Setup(service => service.GetOrdersByCustomerId(customerId)).ReturnsAsync(orders);

        // Act
        var result = await _controller.GetOrderByCustomerId(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedOrders = Assert.IsAssignableFrom<IEnumerable<OrderMain>>(okResult.Value);
        Assert.Single(returnedOrders);
    }

    [Fact]
    public async Task GetOrderByCustomerId_ShouldReturnNotFound_WhenNoOrdersExist()
    {
        // Arrange
        var customerId = 1;
        _mockOrderService.Setup(service => service.GetOrdersByCustomerId(customerId))
            .ReturnsAsync(new List<OrderMain>());

        // Act
        var result = await _controller.GetOrderByCustomerId(customerId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Empty((IEnumerable<OrderMain>)notFoundResult.Value);
    }

    [Fact]
    public async Task GetEntriesForOrder_ShouldReturnOk_WhenEntriesExist()
    {
        // Arrange
        var orderId = 1;
        var orderEntries = new List<OrderEntryQto> { new() { Id = 1, OrderQuantity = 5 } };
        _mockOrderService.Setup(service => service.GetEntriesForOrder(orderId)).ReturnsAsync(orderEntries);

        // Act
        var result = await _controller.GetEntriesForOrder(orderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEntries = Assert.IsAssignableFrom<IEnumerable<OrderEntryQto>>(okResult.Value);
        Assert.Single(returnedEntries);
    }

    [Fact]
    public async Task GetEntriesForOrder_ShouldReturnNotFound_WhenNoEntriesExist()
    {
        // Arrange
        var orderId = 1;
        _mockOrderService.Setup(service => service.GetEntriesForOrder(orderId)).ReturnsAsync(new List<OrderEntryQto>());

        // Act
        var result = await _controller.GetEntriesForOrder(orderId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Empty((IEnumerable<OrderEntryQto>)notFoundResult.Value);
    }

    [Fact]
    public async Task ChangeOrderStatus_ShouldReturnOk_WhenStatusIsModified()
    {
        // Arrange
        var orderId = new IdentificationDto { Id = 1 };
        var status = "Shipped";
        _mockOrderService.Setup(service => service.ModifyOrderStatus(orderId.Id, status)).ReturnsAsync(true);

        // Act
        var result = await _controller.ChangeOrderStatus(orderId, status);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.True((bool)okResult.Value);
    }

    [Fact]
    public async Task ChangeOrderStatus_ShouldReturnBadRequest_WhenStatusIsNotModified()
    {
        // Arrange
        var orderId = new IdentificationDto { Id = 1 };
        var status = "Shipped";
        _mockOrderService.Setup(service => service.ModifyOrderStatus(orderId.Id, status)).ReturnsAsync(false);

        // Act
        var result = await _controller.ChangeOrderStatus(orderId, status);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.False((bool)badRequestResult.Value);
    }

    [Fact]
    public async Task GetCustomerOrderHistory_ShouldReturnOk_WhenHistoryExists()
    {
        // Arrange
        var customerId = 1;
        var orderHistory = new List<OrderMain> { new() { Id = 1, Status = "Completed" } };
        _mockOrderService.Setup(service => service.GetCustomerOrderHistory(customerId)).ReturnsAsync(orderHistory);

        // Act
        var result = await _controller.GetCustomerOrderHistory(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedHistory = Assert.IsAssignableFrom<IEnumerable<OrderMain>>(okResult.Value);
        Assert.Single(returnedHistory);
    }

    [Fact]
    public async Task PlaceOrder_ShouldReturnOrder_WhenOrderIsPlacedSuccessfully()
    {
        // Arrange
        var customerId = 1;
        var orderPlacedDto = new OrderPlacedDto
        {
            OrderPlacedProducts = new List<OrderEntryPlacedDto>
            {
                new() { ProductId = 1, Quantity = 2 }
            }
        };
        var placedOrder = new OrderMain { Id = 1, Status = "Pending" };
        _mockOrderService.Setup(service => service.PlaceOrder(customerId, It.IsAny<List<OrderEntryPlaced>>()))
            .ReturnsAsync(placedOrder);

        // Act
        var result = await _controller.PlaceOrder(customerId, orderPlacedDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<OrderMain>>(result);
        var returnedOrder = Assert.IsType<OrderMain>(actionResult.Value);
        Assert.Equal(placedOrder.Id, returnedOrder.Id);
        Assert.Equal(placedOrder.Status, returnedOrder.Status);
    }
}