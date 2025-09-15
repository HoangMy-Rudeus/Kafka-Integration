namespace KafkaMicroservices.Shared.Domain.Events;

/// <summary>
/// Base interface for all domain events
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
}

/// <summary>
/// Abstract base class for domain events
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    public Guid EventId { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public abstract string EventType { get; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }

    protected DomainEvent(Guid eventId, DateTime occurredOn)
    {
        EventId = eventId;
        OccurredOn = occurredOn;
    }
}