using KafkaMicroservices.Shared.Domain.Entities;
using KafkaMicroservices.Shared.Domain.ValueObjects;
using KafkaMicroservices.Shared.Domain.Events;

namespace KafkaMicroservices.Shared.Domain.Entities;

/// <summary>
/// Notification domain entity representing customer notifications
/// </summary>
public class Notification : BaseEntity
{
    public CustomerId CustomerId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Message { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    // Required for EF Core
    private Notification() : base()
    {
        CustomerId = CustomerId.Empty;
        Type = NotificationType.Info;
        Message = string.Empty;
        IsRead = false;
    }

    public Notification(CustomerId customerId, NotificationType type, string message) : base()
    {
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
        Type = type;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        IsRead = false;

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));

        AddDomainEvent(new NotificationCreatedDomainEvent(Id, CustomerId, Type, Message));
    }

    public void MarkAsRead()
    {
        if (IsRead) return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;
        SetUpdatedAt();

        AddDomainEvent(new NotificationReadDomainEvent(Id, CustomerId));
    }

    public void MarkAsUnread()
    {
        if (!IsRead) return;

        IsRead = false;
        ReadAt = null;
        SetUpdatedAt();

        AddDomainEvent(new NotificationUnreadDomainEvent(Id, CustomerId));
    }

    public static Notification CreateOrderConfirmation(CustomerId customerId, Guid orderId, Money totalAmount)
    {
        var message = $"Your order {orderId} has been confirmed. Total amount: {totalAmount}";
        return new Notification(customerId, NotificationType.OrderConfirmation, message);
    }

    public static Notification CreateInventoryReservation(CustomerId customerId, Guid orderId)
    {
        var message = $"Inventory has been reserved for your order {orderId}. Your order is being processed.";
        return new Notification(customerId, NotificationType.InventoryReservation, message);
    }

    public static Notification CreateOrderCancellation(CustomerId customerId, Guid orderId, string reason)
    {
        var message = $"Your order {orderId} has been cancelled. Reason: {reason}";
        return new Notification(customerId, NotificationType.OrderCancellation, message);
    }

    public static Notification CreateOrderCompletion(CustomerId customerId, Guid orderId)
    {
        var message = $"Your order {orderId} has been completed and is ready for delivery.";
        return new Notification(customerId, NotificationType.OrderCompletion, message);
    }
}

/// <summary>
/// Notification type enumeration
/// </summary>
public enum NotificationType
{
    Info = 0,
    OrderConfirmation = 1,
    InventoryReservation = 2,
    OrderCancellation = 3,
    OrderCompletion = 4,
    StockAlert = 5,
    SystemAlert = 6
}