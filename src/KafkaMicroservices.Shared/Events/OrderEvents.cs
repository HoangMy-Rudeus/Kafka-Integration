using KafkaMicroservices.Shared.Models;

namespace KafkaMicroservices.Shared.Events;

public abstract class BaseEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = string.Empty;
}

public class OrderCreatedEvent : BaseEvent
{
    public OrderCreatedEvent()
    {
        EventType = nameof(OrderCreatedEvent);
    }
    
    public Order Order { get; set; } = new();
}

public class OrderConfirmedEvent : BaseEvent
{
    public OrderConfirmedEvent()
    {
        EventType = nameof(OrderConfirmedEvent);
    }
    
    public Guid OrderId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class InventoryReservedEvent : BaseEvent
{
    public InventoryReservedEvent()
    {
        EventType = nameof(InventoryReservedEvent);
    }
    
    public Guid OrderId { get; set; }
    public List<ReservedItem> ReservedItems { get; set; } = new();
}

public class ReservedItem
{
    public string ProductId { get; set; } = string.Empty;
    public int QuantityReserved { get; set; }
}

public class NotificationSentEvent : BaseEvent
{
    public NotificationSentEvent()
    {
        EventType = nameof(NotificationSentEvent);
    }
    
    public string CustomerId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
}
