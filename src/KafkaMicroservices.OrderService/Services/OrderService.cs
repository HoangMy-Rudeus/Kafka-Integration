using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Models;

namespace KafkaMicroservices.OrderService.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
    Task<Order?> GetOrderAsync(Guid orderId);
    Task<IEnumerable<Order>> GetOrdersAsync();
}

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly List<Order> _orders = new(); // In-memory storage for demo

    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Items = request.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList(),
            TotalAmount = request.Items.Sum(item => item.Price * item.Quantity),
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        _orders.Add(order);
        _logger.LogInformation("Created order {OrderId} for customer {CustomerId}", order.Id, order.CustomerId);

        return await Task.FromResult(order);
    }

    public async Task<Order?> GetOrderAsync(Guid orderId)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        return await Task.FromResult(order);
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return await Task.FromResult(_orders.AsEnumerable());
    }
}

public class CreateOrderRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public List<CreateOrderItem> Items { get; set; } = new();
}

public class CreateOrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
