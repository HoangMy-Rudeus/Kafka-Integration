using KafkaMicroservices.InventoryService.Data;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Models;
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

    public InventoryService(ILogger<InventoryService> logger, InventoryDbContext context)
    {
        _logger = logger;
        _context = context;
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
