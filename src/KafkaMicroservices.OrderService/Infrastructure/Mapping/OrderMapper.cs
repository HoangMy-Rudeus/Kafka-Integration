using KafkaMicroservices.Shared.Domain.ValueObjects;
using DomainOrder = KafkaMicroservices.Shared.Domain.Entities.Order;
using DomainOrderItem = KafkaMicroservices.Shared.Domain.Entities.OrderItem;
using DbOrder = KafkaMicroservices.Shared.Models.Order;
using DbOrderItem = KafkaMicroservices.Shared.Models.OrderItem;

namespace KafkaMicroservices.OrderService.Infrastructure.Mapping;

/// <summary>
/// Mapper between database models and domain entities
/// </summary>
public static class OrderMapper
{
    /// <summary>
    /// Maps from database model to domain entity
    /// </summary>
    public static DomainOrder ToDomainEntity(DbOrder dbOrder)
    {
        if (dbOrder == null) throw new ArgumentNullException(nameof(dbOrder));

        // Create order items
        var domainItems = dbOrder.Items.Select(item => new DomainOrderItem(
            new ProductId(item.ProductId),
            new ProductName(item.ProductName),
            new Quantity(item.Quantity),
            new Money(item.Price)
        )).ToList();

        // Create the domain order using reflection to set the ID and timestamps
        // since the constructor creates a new ID
        var domainOrder = new DomainOrder(
            new CustomerId(dbOrder.CustomerId),
            domainItems
        );

        // Use reflection to set the private fields to match the database values
        SetPrivateProperty(domainOrder, "Id", dbOrder.Id);
        SetPrivateProperty(domainOrder, "CreatedAt", dbOrder.CreatedAt);
        
        // Set status using reflection since it's not in constructor
        SetPrivateProperty(domainOrder, "Status", (KafkaMicroservices.Shared.Domain.Entities.OrderStatus)dbOrder.Status);

        return domainOrder;
    }

    /// <summary>
    /// Maps from domain entity to database model
    /// </summary>
    public static DbOrder ToDbModel(DomainOrder domainOrder)
    {
        if (domainOrder == null) throw new ArgumentNullException(nameof(domainOrder));

        return new DbOrder
        {
            Id = domainOrder.Id,
            CustomerId = domainOrder.CustomerId.Value,
            Items = domainOrder.Items.Select(item => new DbOrderItem
            {
                ProductId = item.ProductId.Value,
                ProductName = item.ProductName.Value,
                Quantity = item.Quantity.Value,
                Price = item.UnitPrice.Amount
            }).ToList(),
            TotalAmount = domainOrder.TotalAmount.Amount,
            CreatedAt = domainOrder.CreatedAt,
            Status = (KafkaMicroservices.Shared.Models.OrderStatus)domainOrder.Status
        };
    }

    /// <summary>
    /// Updates database model from domain entity (for updates)
    /// </summary>
    public static void UpdateDbModel(DbOrder dbOrder, DomainOrder domainOrder)
    {
        if (dbOrder == null) throw new ArgumentNullException(nameof(dbOrder));
        if (domainOrder == null) throw new ArgumentNullException(nameof(domainOrder));

        dbOrder.CustomerId = domainOrder.CustomerId.Value;
        dbOrder.TotalAmount = domainOrder.TotalAmount.Amount;
        dbOrder.Status = (KafkaMicroservices.Shared.Models.OrderStatus)domainOrder.Status;
        
        // Update items
        dbOrder.Items.Clear();
        dbOrder.Items.AddRange(domainOrder.Items.Select(item => new DbOrderItem
        {
            ProductId = item.ProductId.Value,
            ProductName = item.ProductName.Value,
            Quantity = item.Quantity.Value,
            Price = item.UnitPrice.Amount
        }));

        // Update timestamps if they exist
        if (domainOrder.UpdatedAt.HasValue)
        {
            // Note: We might need to add UpdatedAt to the DbOrder model
            // For now, we'll set CreatedAt which exists
        }
    }

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var type = obj.GetType();
        var property = type.GetProperty(propertyName, 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance);
        
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            // Try to set via backing field
            var field = type.GetField($"<{propertyName}>k__BackingField", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            field?.SetValue(obj, value);
        }
    }
}