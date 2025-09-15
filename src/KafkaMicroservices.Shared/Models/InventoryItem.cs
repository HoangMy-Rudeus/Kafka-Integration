namespace KafkaMicroservices.Shared.Models;

/// <summary>
/// Represents an inventory item in the inventory management system
/// </summary>
public class InventoryItem
{
    /// <summary>
    /// Unique product identifier
    /// </summary>
    public string ProductId { get; set; } = string.Empty;
    
    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
    
    /// <summary>
    /// Available quantity in stock
    /// </summary>
    public int AvailableQuantity { get; set; }
    
    /// <summary>
    /// Quantity reserved for pending orders
    /// </summary>
    public int ReservedQuantity { get; set; }
}
