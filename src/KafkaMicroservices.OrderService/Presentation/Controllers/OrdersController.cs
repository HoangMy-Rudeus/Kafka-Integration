using KafkaMicroservices.Shared.Application.Interfaces;
using KafkaMicroservices.Shared.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KafkaMicroservices.OrderService.Presentation.Controllers;

/// <summary>
/// Orders API controller following clean architecture principles
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderApplicationService _orderApplicationService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderApplicationService orderApplicationService,
        ILogger<OrdersController> logger)
    {
        _orderApplicationService = orderApplicationService ?? throw new ArgumentNullException(nameof(orderApplicationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="request">Order creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);

            var command = new CreateOrderCommand(
                new CustomerId(request.CustomerId),
                request.Items.Select(item => new CreateOrderItemCommand(
                    new ProductId(item.ProductId),
                    new ProductName(item.ProductName ?? item.ProductId), // Fallback to ProductId if name is empty
                    new Quantity(item.Quantity),
                    new Money(item.UnitPrice)
                ))
            );

            var order = await _orderApplicationService.CreateOrderAsync(command, cancellationToken);

            var response = MapToResponse(order);
            
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for creating order");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for customer {CustomerId}", request.CustomerId);
            return StatusCode(500, "An error occurred while creating the order");
        }
    }

    /// <summary>
    /// Gets an order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetOrder(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderApplicationService.GetOrderAsync(id, cancellationToken);
        
        if (order == null)
        {
            return NotFound($"Order {id} not found");
        }

        return Ok(MapToResponse(order));
    }

    /// <summary>
    /// Gets all orders for a customer
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of customer orders</returns>
    [HttpGet("customer/{customerId}")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetCustomerOrders(
        string customerId, 
        CancellationToken cancellationToken = default)
    {
        var orders = await _orderApplicationService.GetCustomerOrdersAsync(new CustomerId(customerId), cancellationToken);
        var responses = orders.Select(MapToResponse);
        
        return Ok(responses);
    }

    /// <summary>
    /// Confirms an order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmOrder(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _orderApplicationService.ConfirmOrderAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Cancels an order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="request">Cancellation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelOrder(
        Guid id, 
        [FromBody] CancelOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _orderApplicationService.CancelOrderAsync(id, request.Reason, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private static OrderResponse MapToResponse(KafkaMicroservices.Shared.Domain.Entities.Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId.Value,
            Items = order.Items.Select(item => new OrderItemResponse
            {
                ProductId = item.ProductId.Value,
                ProductName = item.ProductName.Value,
                Quantity = item.Quantity.Value,
                UnitPrice = item.UnitPrice.Amount,
                LineTotal = item.LineTotal.Amount
            }).ToList(),
            TotalAmount = order.TotalAmount.Amount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}

// Request/Response DTOs
public class CreateOrderRequest
{
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "Order must have at least one item")]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    public string? ProductName { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }
}

public class CancelOrderRequest
{
    [Required]
    public string Reason { get; set; } = string.Empty;
}

public class OrderResponse
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public List<OrderItemResponse> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OrderItemResponse
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}