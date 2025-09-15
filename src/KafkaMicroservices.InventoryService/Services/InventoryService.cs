using KafkaMicroservices.InventoryService.Data;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Models;
using KafkaMicroservices.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace KafkaMicroservices.InventoryService.Services;

public interface IInventoryService
{
    Task<bool> ReserveInventoryAsync(Guid orderId, List<OrderItem> items);
    Task<InventoryItem?> GetInventoryAsync(string productId);
    Task<IEnumerable<InventoryItem>> GetAllInventoryAsync();
}

public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    private readonly InventoryDbContext _context;
    private readonly IKafkaProducer<BaseEvent> _kafkaProducer;

    public InventoryService(ILogger<InventoryService> logger, InventoryDbContext context, IKafkaProducer<BaseEvent> kafkaProducer)
    {
        _logger = logger;
        _context = context;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<bool> ReserveInventoryAsync(Guid orderId, List<OrderItem> items)
    {
        _logger.LogInformation("Attempting to reserve inventory for order {OrderId}", orderId);
        
        var canReserveAll = true;
        var reservations = new List<(InventoryItem Item, int Quantity)>();

        // Check if all items can be reserved
        foreach (var orderItem in items)
        {
            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == orderItem.ProductId);
            
            if (inventoryItem == null)
            {
                _logger.LogWarning("Product {ProductId} not found in inventory", orderItem.ProductId);
                canReserveAll = false;
                break;
            }

            if (inventoryItem.AvailableQuantity < orderItem.Quantity)
            {
                _logger.LogWarning("Insufficient inventory for product {ProductId}. Available: {Available}, Requested: {Requested}",
                    orderItem.ProductId, inventoryItem.AvailableQuantity, orderItem.Quantity);
                canReserveAll = false;
                break;
            }

            reservations.Add((inventoryItem, orderItem.Quantity));
        }

        if (canReserveAll)
        {
            // Apply all reservations
            foreach (var (item, quantity) in reservations)
            {
                item.AvailableQuantity -= quantity;
                item.ReservedQuantity += quantity;
                _logger.LogInformation("Reserved {Quantity} units of {ProductId} for order {OrderId}",
                    quantity, item.ProductId, orderId);
            }

            await _context.SaveChangesAsync();
            
            // Publish inventory reserved event
            var inventoryReservedEvent = new InventoryReservedEvent
            {
                EventId = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                OrderId = orderId,
                ReservedItems = items.Select(item => new ReservedItem
                {
                    ProductId = item.ProductId,
                    QuantityReserved = item.Quantity
                }).ToList()
            };

            try
            {
                await _kafkaProducer.ProduceAsync("inventory-reserved", inventoryReservedEvent);
                _logger.LogInformation("Published inventory reserved event for order {OrderId}", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish inventory reserved event for order {OrderId}", orderId);
                // Note: We don't throw here to avoid rolling back the database transaction
                // In a production system, you might want to implement a reliable outbox pattern
            }

            _logger.LogInformation("Successfully reserved inventory for order {OrderId}", orderId);
            return true;
        }

        _logger.LogWarning("Failed to reserve inventory for order {OrderId}", orderId);
        return false;
    }

    public async Task<InventoryItem?> GetInventoryAsync(string productId)
    {
        return await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public async Task<IEnumerable<InventoryItem>> GetAllInventoryAsync()
    {
        return await _context.InventoryItems.ToListAsync();
    }
}
