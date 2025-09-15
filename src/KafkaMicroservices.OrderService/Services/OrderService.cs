using KafkaMicroservices.OrderService.Data;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KafkaMicroservices.OrderService.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
    Task<Order?> GetOrderAsync(Guid id);
    Task<IEnumerable<Order>> GetOrdersAsync();
}

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly OrderDbContext _context;

    public OrderService(ILogger<OrderService> logger, OrderDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Items = request.Items,
            TotalAmount = request.Items.Sum(item => item.Price * item.Quantity),
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Order {OrderId} created for customer {CustomerId} with total amount {TotalAmount}", 
            order.Id, order.CustomerId, order.TotalAmount);

        return order;
    }

    public async Task<Order?> GetOrderAsync(Guid id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        return order;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return await _context.Orders.ToListAsync();
    }
}

/// <summary>
/// Request model for creating a new order
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// Customer ID placing the order
    /// </summary>
    [Required(ErrorMessage = "Customer ID is required")]
    public string CustomerId { get; set; } = string.Empty;
    
    /// <summary>
    /// List of items in the order
    /// </summary>
    [Required(ErrorMessage = "Items are required")]
    [MinLength(1, ErrorMessage = "At least one item is required")]
    public List<OrderItem> Items { get; set; } = new();
}
