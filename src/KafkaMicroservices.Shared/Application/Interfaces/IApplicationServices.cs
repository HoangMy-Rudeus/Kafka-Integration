using KafkaMicroservices.Shared.Domain.Entities;
using KafkaMicroservices.Shared.Domain.ValueObjects;

namespace KafkaMicroservices.Shared.Application.Interfaces;

/// <summary>
/// Application service interface for order management
/// </summary>
public interface IOrderApplicationService
{
    Task<Order> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken = default);
    Task<Order?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetCustomerOrdersAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task ConfirmOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task CompleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task CancelOrderAsync(Guid orderId, string reason, CancellationToken cancellationToken = default);
}

/// <summary>
/// Application service interface for inventory management
/// </summary>
public interface IInventoryApplicationService
{
    Task<InventoryItem?> GetInventoryAsync(ProductId productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItem>> GetAllInventoryAsync(CancellationToken cancellationToken = default);
    Task<bool> ReserveInventoryAsync(ReserveInventoryCommand command, CancellationToken cancellationToken = default);
    Task ReleaseInventoryAsync(ReleaseInventoryCommand command, CancellationToken cancellationToken = default);
    Task FulfillInventoryAsync(FulfillInventoryCommand command, CancellationToken cancellationToken = default);
    Task AdjustStockAsync(AdjustStockCommand command, CancellationToken cancellationToken = default);
    Task RestockAsync(RestockCommand command, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync(int threshold, CancellationToken cancellationToken = default);
}

/// <summary>
/// Application service interface for notification management
/// </summary>
public interface INotificationApplicationService
{
    Task<Notification> SendNotificationAsync(SendNotificationCommand command, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetCustomerNotificationsAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkAsUnreadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(CustomerId customerId, CancellationToken cancellationToken = default);
}

// Command objects for better encapsulation
public record CreateOrderCommand(CustomerId CustomerId, IEnumerable<CreateOrderItemCommand> Items);
public record CreateOrderItemCommand(ProductId ProductId, ProductName ProductName, Quantity Quantity, Money UnitPrice);

public record ReserveInventoryCommand(ProductId ProductId, Quantity Quantity, Guid OrderId);
public record ReleaseInventoryCommand(ProductId ProductId, Quantity Quantity, Guid OrderId);
public record FulfillInventoryCommand(ProductId ProductId, Quantity Quantity, Guid OrderId);
public record AdjustStockCommand(ProductId ProductId, Quantity NewQuantity, string Reason);
public record RestockCommand(ProductId ProductId, Quantity Quantity, string Source);

public record SendNotificationCommand(CustomerId CustomerId, NotificationType Type, string Message);