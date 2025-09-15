using KafkaMicroservices.Shared.Domain.Events;
using KafkaMicroservices.Shared.Domain.ValueObjects;
using KafkaMicroservices.Shared.Domain.Entities;

namespace KafkaMicroservices.Shared.Domain.Events;

/// <summary>
/// Domain event raised when a notification is created
/// </summary>
public class NotificationCreatedDomainEvent : DomainEvent
{
    public Guid NotificationId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Message { get; private set; }

    public override string EventType => nameof(NotificationCreatedDomainEvent);

    public NotificationCreatedDomainEvent(Guid notificationId, CustomerId customerId, NotificationType type, string message)
        : base()
    {
        NotificationId = notificationId;
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
        Type = type;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}

/// <summary>
/// Domain event raised when a notification is read
/// </summary>
public class NotificationReadDomainEvent : DomainEvent
{
    public Guid NotificationId { get; private set; }
    public CustomerId CustomerId { get; private set; }

    public override string EventType => nameof(NotificationReadDomainEvent);

    public NotificationReadDomainEvent(Guid notificationId, CustomerId customerId)
        : base()
    {
        NotificationId = notificationId;
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
    }
}

/// <summary>
/// Domain event raised when a notification is marked as unread
/// </summary>
public class NotificationUnreadDomainEvent : DomainEvent
{
    public Guid NotificationId { get; private set; }
    public CustomerId CustomerId { get; private set; }

    public override string EventType => nameof(NotificationUnreadDomainEvent);

    public NotificationUnreadDomainEvent(Guid notificationId, CustomerId customerId)
        : base()
    {
        NotificationId = notificationId;
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
    }
}