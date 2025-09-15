using KafkaMicroservices.Shared.Domain.Events;

namespace KafkaMicroservices.Shared.Application.Interfaces;

/// <summary>
/// Interface for publishing domain events to external messaging systems
/// </summary>
public interface IDomainEventPublisher
{
    /// <summary>
    /// Publishes a domain event to the messaging system
    /// </summary>
    Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent;

    /// <summary>
    /// Publishes multiple domain events to the messaging system
    /// </summary>
    Task PublishManyAsync<TEvent>(IEnumerable<TEvent> domainEvents, CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent;
}

/// <summary>
/// Interface for handling domain events
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the specified domain event
    /// </summary>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for dispatching domain events to their handlers
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches a domain event to all registered handlers
    /// </summary>
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent;

    /// <summary>
    /// Dispatches multiple domain events to their handlers
    /// </summary>
    Task DispatchManyAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}