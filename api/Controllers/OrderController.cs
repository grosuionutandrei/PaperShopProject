﻿using api.TransferModels;
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
    public async Task<ActionResult<IEnumerable<OrderMain>>> GetOrderByCustomerId([FromRoute] int customerId)
    {
        var results = await _orderService.GetOrdersByCustomerId(customerId);
        if (!results.Any())
        {
            return NotFound(results);
        }
        return Ok(results);
    }


    [HttpGet]
    [Route("/api/order/{orderId}/orderentries")]
    public async Task<ActionResult<IEnumerable<OrderEntryQto>>> GetEntriesForOrder([FromRoute ] int orderId)
    {
        var results = await _orderService.GetEntriesForOrder(orderId);
        if (!results.Any())
        {
            return NotFound(results);
        }
        return Ok(results);
    }

    [HttpPatch]
    [Route("/api/order/edit/{orderId}")]
    public async Task<ActionResult<bool>> ChangeOrderStatus([FromRoute] IdentificationDto orderId,[FromQuery] string status)
    {
        var statusModified = await _orderService.ModifyOrderStatus(orderId.Id,status);
        if (!statusModified)
        {
            return BadRequest(statusModified);
        }
        return Ok(statusModified);
    }

    [HttpPost]
    [Route("/api/customer/{customerId}/placeOrder")]
    public async Task<ActionResult<OrderMain>> PlaceOrder([FromRoute] int customerId,[FromBody] OrderPlacedDto orderPlaced)
    {
        var orderPlacedEntries = orderPlaced.OrderPlacedProducts!
            .Select(e => new OrderEntryPlaced
            {
                ProductId = e.ProductId,
                Quantity = e.Quantity
            }).ToList();
        var placeOrder = await _orderService.PlaceOrder(customerId,orderPlacedEntries);
        return placeOrder;
    }
    
    
    /// <summary>
    /// Get order history for a customer.
    /// </summary>
    /// <value>.</value>
    /// 
    [HttpGet]
    [Route("/customer/{customerId}/history")]
    public async Task<ActionResult<IEnumerable<OrderMain>>> GetCustomerOrderHistory([FromRoute] int customerId )
    {
        var customerHistory =await _orderService.GetCustomerOrderHistory(customerId);
        return Ok(customerHistory );
    }
    
    
}