using KafkaMicroservices.Shared.Domain.Entities;
using KafkaMicroservices.Shared.Domain.ValueObjects;

namespace KafkaMicroservices.Shared.Application.Interfaces;

/// <summary>
/// Repository interface for Order aggregate
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Order order, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for InventoryItem aggregate
/// </summary>
public interface IInventoryRepository
{
    Task<InventoryItem?> GetByProductIdAsync(ProductId productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<InventoryItem> AddAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(ProductId productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync(int threshold, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Notification aggregate
/// </summary>
public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUnreadByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task DeleteAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(CustomerId customerId, CancellationToken cancellationToken = default);
}