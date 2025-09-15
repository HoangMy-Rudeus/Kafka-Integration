using KafkaMicroservices.OrderService.Services;
using KafkaMicroservices.Shared.Configuration;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KafkaMicroservices.OrderService.Controllers;

/// <summary>
/// API for managing orders in the microservices demo
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IKafkaProducer<BaseEvent> _kafkaProducer;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderService orderService,
        IKafkaProducer<BaseEvent> kafkaProducer,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="request">Order creation request</param>
    /// <returns>The created order</returns>
    /// <response code="201">Order created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(request);
            
            // Publish OrderCreatedEvent to Kafka
            var orderCreatedEvent = new OrderCreatedEvent
            {
                Order = order
            };

            await _kafkaProducer.ProduceAsync(Topics.OrderCreated, orderCreatedEvent);
            
            _logger.LogInformation("Order {OrderId} created and event published", order.Id);
            
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets an order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>The order details</returns>
    /// <response code="200">Order found</response>
    /// <response code="404">Order not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder([Required] Guid id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        
        return Ok(order);
    }

    /// <summary>
    /// Gets all orders
    /// </summary>
    /// <returns>List of orders</returns>
    /// <response code="200">Orders retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }
}
