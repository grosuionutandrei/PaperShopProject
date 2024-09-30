using infrastructure.QuerryModels;
using Microsoft.AspNetCore.Mvc;
using service.Orders;

namespace api.Controllers;
[ApiController]
public class OrderController:ControllerBase
{
    private IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService service,ILogger<OrderController> logger)
    {
        _orderService = service;
        _logger = logger;
    }

    [HttpGet]
    [Route("/api/customer/{customerId}/orders")]
    public async Task<ActionResult<IEnumerable<OrderMain>>> GetOrderByCustomerId(int customerId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var results = await _orderService.GetOrdersByCustomerId(customerId);
        if (!results.Any())
        {
            return NotFound(results);
        }
        return Ok(results);
    }


    [HttpGet]
    [Route("/api/order/{orderId}/orderentries")]
    public async Task<ActionResult<IEnumerable<OrderEntry>>> GetEntriesForOrder(int orderId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var results = await _orderService.GetEntriesForOrder(orderId);
        if (!results.Any())
        {
            return NotFound(results);
        }
        return Ok(results);
    }

    [HttpPatch]
    [Route("/api/order/edit/{orderId}")]
    public async Task<ActionResult<bool>> ChangeOrderStatus(int orderId,[FromQuery] string status)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var statusModified = await _orderService.ModifyOrderStatus(orderId,status);
        if (!statusModified)
        {
            return BadRequest(statusModified);
        }

        return Ok(statusModified);
    }

    [HttpPost]
    [Route("/api/customer/{customerId}/placeOrder")]
    public async Task<ActionResult<bool>> PlaceOrder()
    {
    }




}