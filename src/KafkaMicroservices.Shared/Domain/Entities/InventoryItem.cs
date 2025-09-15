using KafkaMicroservices.Shared.Domain.Entities;
using KafkaMicroservices.Shared.Domain.ValueObjects;
using KafkaMicroservices.Shared.Domain.Events;

namespace KafkaMicroservices.Shared.Domain.Entities;

/// <summary>
/// Inventory item domain entity representing product stock
/// </summary>
public class InventoryItem : BaseEntity
{
    public ProductId ProductId { get; private set; }
    public ProductName ProductName { get; private set; }
    public Quantity AvailableQuantity { get; private set; }
    public Quantity ReservedQuantity { get; private set; }
    public Quantity TotalQuantity => new Quantity(AvailableQuantity.Value + ReservedQuantity.Value);

    // Required for EF Core
    private InventoryItem() : base()
    {
        ProductId = ProductId.Empty;
        ProductName = ProductName.Empty;
        AvailableQuantity = Quantity.Zero;
        ReservedQuantity = Quantity.Zero;
    }

    public InventoryItem(ProductId productId, ProductName productName, Quantity initialQuantity) : base()
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        AvailableQuantity = initialQuantity ?? throw new ArgumentNullException(nameof(initialQuantity));
        ReservedQuantity = Quantity.Zero;

        if (initialQuantity.Value < 0)
            throw new ArgumentException("Initial quantity cannot be negative", nameof(initialQuantity));
    }

    public bool CanReserve(Quantity quantity)
    {
        if (quantity == null) throw new ArgumentNullException(nameof(quantity));
        return AvailableQuantity.Value >= quantity.Value;
    }

    public void Reserve(Quantity quantity, Guid orderId)
    {
        if (quantity == null) throw new ArgumentNullException(nameof(quantity));
        if (orderId == Guid.Empty) throw new ArgumentException("Order ID cannot be empty", nameof(orderId));

        if (!CanReserve(quantity))
            throw new InvalidOperationException($"Insufficient inventory. Available: {AvailableQuantity.Value}, Requested: {quantity.Value}");

        AvailableQuantity = new Quantity(AvailableQuantity.Value - quantity.Value);
        ReservedQuantity = new Quantity(ReservedQuantity.Value + quantity.Value);
        SetUpdatedAt();

        AddDomainEvent(new InventoryReservedDomainEvent(ProductId, quantity, orderId));
    }

    public void ReleaseReservation(Quantity quantity, Guid orderId)
    {
        if (quantity == null) throw new ArgumentNullException(nameof(quantity));
        if (orderId == Guid.Empty) throw new ArgumentException("Order ID cannot be empty", nameof(orderId));

        if (ReservedQuantity.Value < quantity.Value)
            throw new InvalidOperationException($"Cannot release more than reserved. Reserved: {ReservedQuantity.Value}, Requested: {quantity.Value}");

        ReservedQuantity = new Quantity(ReservedQuantity.Value - quantity.Value);
        AvailableQuantity = new Quantity(AvailableQuantity.Value + quantity.Value);
        SetUpdatedAt();

        AddDomainEvent(new InventoryReleasedDomainEvent(ProductId, quantity, orderId));
    }

    public void FulfillReservation(Quantity quantity, Guid orderId)
    {
        if (quantity == null) throw new ArgumentNullException(nameof(quantity));
        if (orderId == Guid.Empty) throw new ArgumentException("Order ID cannot be empty", nameof(orderId));

        if (ReservedQuantity.Value < quantity.Value)
            throw new InvalidOperationException($"Cannot fulfill more than reserved. Reserved: {ReservedQuantity.Value}, Requested: {quantity.Value}");

        ReservedQuantity = new Quantity(ReservedQuantity.Value - quantity.Value);
        SetUpdatedAt();

        AddDomainEvent(new InventoryFulfilledDomainEvent(ProductId, quantity, orderId));
    }

    public void AdjustStock(Quantity newAvailableQuantity, string reason)
    {
        if (newAvailableQuantity == null) throw new ArgumentNullException(nameof(newAvailableQuantity));
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason cannot be empty", nameof(reason));

        var oldQuantity = AvailableQuantity;
        AvailableQuantity = newAvailableQuantity;
        SetUpdatedAt();

        AddDomainEvent(new InventoryAdjustedDomainEvent(ProductId, oldQuantity, newAvailableQuantity, reason));
    }

    public void Restock(Quantity quantity, string source)
    {
        if (quantity == null) throw new ArgumentNullException(nameof(quantity));
        if (string.IsNullOrWhiteSpace(source)) throw new ArgumentException("Source cannot be empty", nameof(source));

        if (quantity.Value <= 0)
            throw new ArgumentException("Restock quantity must be greater than zero", nameof(quantity));

        AvailableQuantity = new Quantity(AvailableQuantity.Value + quantity.Value);
        SetUpdatedAt();

        AddDomainEvent(new InventoryRestockedDomainEvent(ProductId, quantity, source));
    }
}