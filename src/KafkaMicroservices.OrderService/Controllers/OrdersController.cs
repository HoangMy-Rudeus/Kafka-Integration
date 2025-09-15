using KafkaMicroservices.OrderService.Services;
using KafkaMicroservices.Shared.Configuration;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace KafkaMicroservices.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpPost]
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        
        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }
}
