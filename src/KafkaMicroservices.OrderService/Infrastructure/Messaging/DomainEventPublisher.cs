using KafkaMicroservices.Shared.Application.Interfaces;
using KafkaMicroservices.Shared.Domain.Events;
using KafkaMicroservices.Shared.Infrastructure.Interfaces;
using KafkaMicroservices.Shared.Events; // For integration events

namespace KafkaMicroservices.OrderService.Infrastructure.Messaging;

/// <summary>
/// Domain event publisher that converts domain events to integration events and publishes them
/// </summary>
public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly IAppLogger<DomainEventPublisher> _logger;

    public DomainEventPublisher(IMessagePublisher messagePublisher, IAppLogger<DomainEventPublisher> logger)
    {
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IDomainEvent
    {
        try
        {
            // Convert domain event to integration event
            var integrationEvent = ConvertToIntegrationEvent(domainEvent);
            if (integrationEvent != null)
            {
                var topic = GetTopicForEvent(domainEvent);
                await _messagePublisher.PublishAsync(topic, domainEvent.EventId.ToString(), integrationEvent, cancellationToken);
                
                _logger.LogInformation("Published domain event {EventType} to topic {Topic}", 
                    domainEvent.EventType, topic);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish domain event {EventType}", domainEvent.EventType);
            throw;
        }
    }

    public async Task PublishManyAsync<TEvent>(IEnumerable<TEvent> domainEvents, CancellationToken cancellationToken = default) where TEvent : IDomainEvent
    {
        var tasks = domainEvents.Select(evt => PublishAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private BaseEvent? ConvertToIntegrationEvent(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            OrderCreatedDomainEvent orderCreated => new OrderCreatedEvent
            {
                EventId = orderCreated.EventId,
                Timestamp = orderCreated.OccurredOn,
                EventType = nameof(OrderCreatedEvent),
                Order = new KafkaMicroservices.Shared.Models.Order
                {
                    Id = orderCreated.OrderId,
                    CustomerId = orderCreated.CustomerId.Value,
                    Items = orderCreated.Items.Select(item => new KafkaMicroservices.Shared.Models.OrderItem
                    {
                        ProductId = item.ProductId.Value,
                        ProductName = item.ProductName.Value,
                        Quantity = item.Quantity.Value,
                        Price = item.UnitPrice.Amount
                    }).ToList(),
                    TotalAmount = orderCreated.TotalAmount.Amount,
                    CreatedAt = orderCreated.OccurredOn,
                    Status = KafkaMicroservices.Shared.Models.OrderStatus.Pending
                }
            },
            OrderConfirmedDomainEvent orderConfirmed => new OrderConfirmedEvent
            {
                EventId = orderConfirmed.EventId,
                Timestamp = orderConfirmed.OccurredOn,
                EventType = nameof(OrderConfirmedEvent),
                OrderId = orderConfirmed.OrderId,
                CustomerId = orderConfirmed.CustomerId.Value
            },
            _ => null
        };
    }

    private string GetTopicForEvent(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            OrderCreatedDomainEvent => "order-created",
            OrderConfirmedDomainEvent => "order-confirmed",
            OrderCompletedDomainEvent => "order-completed",
            OrderCancelledDomainEvent => "order-cancelled",
            _ => "domain-events"
        };
    }
}

// Add missing event classes for completeness
public class OrderConfirmedEvent : BaseEvent
{
    public OrderConfirmedEvent()
    {
        EventType = nameof(OrderConfirmedEvent);
    }

    public Guid OrderId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}