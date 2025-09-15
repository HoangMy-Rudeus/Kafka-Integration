using KafkaMicroservices.Shared.Infrastructure.Interfaces;
using KafkaMicroservices.Shared.Services;
using KafkaMicroservices.Shared.Events;

namespace KafkaMicroservices.OrderService.Infrastructure.Messaging;

/// <summary>
/// Message publisher implementation using existing Kafka producer
/// </summary>
public class MessagePublisher : IMessagePublisher
{
    private readonly IKafkaProducer<BaseEvent> _kafkaProducer;

    public MessagePublisher(IKafkaProducer<BaseEvent> kafkaProducer)
    {
        _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
    }

    public async Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
    {
        if (message is BaseEvent baseEvent)
        {
            await _kafkaProducer.ProduceAsync(topic, baseEvent, cancellationToken);
        }
        else
        {
            throw new ArgumentException($"Message must be of type {nameof(BaseEvent)}", nameof(message));
        }
    }

    public async Task PublishAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default)
    {
        // For now, use the same implementation as above since current Kafka producer doesn't support keys directly
        await PublishAsync(topic, message, cancellationToken);
    }

    public async Task PublishManyAsync<T>(string topic, IEnumerable<T> messages, CancellationToken cancellationToken = default)
    {
        var tasks = messages.Select(message => PublishAsync(topic, message, cancellationToken));
        await Task.WhenAll(tasks);
    }
}