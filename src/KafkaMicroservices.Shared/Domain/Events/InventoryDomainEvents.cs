using KafkaMicroservices.Shared.Domain.Events;
using KafkaMicroservices.Shared.Domain.ValueObjects;

namespace KafkaMicroservices.Shared.Domain.Events;

/// <summary>
/// Domain event raised when inventory is reserved for an order
/// </summary>
public class InventoryReservedDomainEvent : DomainEvent
{
    public ProductId ProductId { get; private set; }
    public Quantity ReservedQuantity { get; private set; }
    public Guid OrderId { get; private set; }

    public override string EventType => nameof(InventoryReservedDomainEvent);

    public InventoryReservedDomainEvent(ProductId productId, Quantity reservedQuantity, Guid orderId)
        : base()
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        ReservedQuantity = reservedQuantity ?? throw new ArgumentNullException(nameof(reservedQuantity));
        OrderId = orderId;
    }
}

/// <summary>
/// Domain event raised when inventory reservation is released
/// </summary>
public class InventoryReleasedDomainEvent : DomainEvent
{
    public ProductId ProductId { get; private set; }
    public Quantity ReleasedQuantity { get; private set; }
    public Guid OrderId { get; private set; }

    public override string EventType => nameof(InventoryReleasedDomainEvent);

    public InventoryReleasedDomainEvent(ProductId productId, Quantity releasedQuantity, Guid orderId)
        : base()
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        ReleasedQuantity = releasedQuantity ?? throw new ArgumentNullException(nameof(releasedQuantity));
        OrderId = orderId;
    }
}

/// <summary>
/// Domain event raised when inventory reservation is fulfilled
/// </summary>
public class InventoryFulfilledDomainEvent : DomainEvent
{
    public ProductId ProductId { get; private set; }
    public Quantity FulfilledQuantity { get; private set; }
    public Guid OrderId { get; private set; }

    public override string EventType => nameof(InventoryFulfilledDomainEvent);

    public InventoryFulfilledDomainEvent(ProductId productId, Quantity fulfilledQuantity, Guid orderId)
        : base()
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        FulfilledQuantity = fulfilledQuantity ?? throw new ArgumentNullException(nameof(fulfilledQuantity));
        OrderId = orderId;
    }
}

/// <summary>
/// Domain event raised when inventory stock is adjusted
/// </summary>
public class InventoryAdjustedDomainEvent : DomainEvent
{
    public ProductId ProductId { get; private set; }
    public Quantity OldQuantity { get; private set; }
    public Quantity NewQuantity { get; private set; }
    public string Reason { get; private set; }

    public override string EventType => nameof(InventoryAdjustedDomainEvent);

    public InventoryAdjustedDomainEvent(ProductId productId, Quantity oldQuantity, Quantity newQuantity, string reason)
        : base()
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        OldQuantity = oldQuantity ?? throw new ArgumentNullException(nameof(oldQuantity));
        NewQuantity = newQuantity ?? throw new ArgumentNullException(nameof(newQuantity));
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
    }
}

/// <summary>
/// Domain event raised when inventory is restocked
/// </summary>
public class InventoryRestockedDomainEvent : DomainEvent
{
    public ProductId ProductId { get; private set; }
    public Quantity RestockedQuantity { get; private set; }
    public string Source { get; private set; }

    public override string EventType => nameof(InventoryRestockedDomainEvent);

    public InventoryRestockedDomainEvent(ProductId productId, Quantity restockedQuantity, string source)
        : base()
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        RestockedQuantity = restockedQuantity ?? throw new ArgumentNullException(nameof(restockedQuantity));
        Source = source ?? throw new ArgumentNullException(nameof(source));
    }
}