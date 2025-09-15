using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Models;

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
    private readonly List<InventoryItem> _inventory = new();

    public InventoryService(ILogger<InventoryService> logger)
    {
        _logger = logger;
        
        // Initialize some sample inventory
        InitializeSampleInventory();
    }

    private void InitializeSampleInventory()
    {
        _inventory.AddRange(new[]
        {
            new InventoryItem { ProductId = "PROD001", ProductName = "Laptop", AvailableQuantity = 50 },
            new InventoryItem { ProductId = "PROD002", ProductName = "Mouse", AvailableQuantity = 100 },
            new InventoryItem { ProductId = "PROD003", ProductName = "Keyboard", AvailableQuantity = 75 },
            new InventoryItem { ProductId = "PROD004", ProductName = "Monitor", AvailableQuantity = 30 }
        });
    }

    public async Task<bool> ReserveInventoryAsync(Guid orderId, List<OrderItem> items)
    {
        _logger.LogInformation("Attempting to reserve inventory for order {OrderId}", orderId);
        
        var canReserveAll = true;
        var reservations = new List<(InventoryItem Item, int Quantity)>();

        // Check if all items can be reserved
        foreach (var orderItem in items)
        {
            var inventoryItem = _inventory.FirstOrDefault(i => i.ProductId == orderItem.ProductId);
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
            // Reserve the inventory
            foreach (var (item, quantity) in reservations)
            {
                item.AvailableQuantity -= quantity;
                item.ReservedQuantity += quantity;
                _logger.LogInformation("Reserved {Quantity} of {ProductId}", quantity, item.ProductId);
            }
            
            _logger.LogInformation("Successfully reserved inventory for order {OrderId}", orderId);
            return true;
        }

        _logger.LogWarning("Failed to reserve inventory for order {OrderId}", orderId);
        return false;
    }

    public async Task<InventoryItem?> GetInventoryAsync(string productId)
    {
        var item = _inventory.FirstOrDefault(i => i.ProductId == productId);
        return await Task.FromResult(item);
    }

    public async Task<IEnumerable<InventoryItem>> GetAllInventoryAsync()
    {
        return await Task.FromResult(_inventory.AsEnumerable());
    }
}

public class InventoryItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
}
