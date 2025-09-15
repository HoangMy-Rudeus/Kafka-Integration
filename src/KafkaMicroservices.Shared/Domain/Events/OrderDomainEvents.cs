using KafkaMicroservices.Shared.Domain.Events;
using KafkaMicroservices.Shared.Domain.ValueObjects;
using KafkaMicroservices.Shared.Domain.Entities;

namespace KafkaMicroservices.Shared.Domain.Events;

/// <summary>
/// Domain event raised when an order is created
/// </summary>
public class OrderCreatedDomainEvent : DomainEvent
{
    public Guid OrderId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public IReadOnlyCollection<OrderItem> Items { get; private set; }
    public Money TotalAmount { get; private set; }

    public override string EventType => nameof(OrderCreatedDomainEvent);

    public OrderCreatedDomainEvent(Guid orderId, CustomerId customerId, ICollection<OrderItem> items, Money totalAmount)
        : base()
    {
        OrderId = orderId;
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
        Items = items?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(items));
        TotalAmount = totalAmount ?? throw new ArgumentNullException(nameof(totalAmount));
    }
}

/// <summary>
/// Domain event raised when an order is confirmed
/// </summary>
public class OrderConfirmedDomainEvent : DomainEvent
{
    public Guid OrderId { get; private set; }
    public CustomerId CustomerId { get; private set; }

    public override string EventType => nameof(OrderConfirmedDomainEvent);

    public OrderConfirmedDomainEvent(Guid orderId, CustomerId customerId)
        : base()
    {
        OrderId = orderId;
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
    }
}

/// <summary>
/// Domain event raised when an order is completed
/// </summary>
public class OrderCompletedDomainEvent : DomainEvent
{
    public Guid OrderId { get; private set; }
    public CustomerId CustomerId { get; private set; }

    public override string EventType => nameof(OrderCompletedDomainEvent);

    public OrderCompletedDomainEvent(Guid orderId, CustomerId customerId)
        : base()
    {
        OrderId = orderId;
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
    }
}

/// <summary>
/// Domain event raised when an order is cancelled
/// </summary>
public class OrderCancelledDomainEvent : DomainEvent
{
    public Guid OrderId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public string Reason { get; private set; }

    public override string EventType => nameof(OrderCancelledDomainEvent);

    public OrderCancelledDomainEvent(Guid orderId, CustomerId customerId, string reason)
        : base()
    {
        OrderId = orderId;
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
        Reason = reason ?? throw new ArgumentNullException(nameof(reason));
    }
}