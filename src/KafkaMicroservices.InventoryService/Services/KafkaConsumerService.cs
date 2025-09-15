using KafkaMicroservices.Shared.Configuration;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Services;
using Confluent.Kafka;
using System.Text.Json;

namespace KafkaMicroservices.InventoryService.Services;

public class KafkaConsumerService : IKafkaConsumer<BaseEvent>, IDisposable
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly JsonSerializerOptions _jsonOptions;
    private CancellationTokenSource? _cancellationTokenSource;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger, 
        KafkaSettings kafkaSettings,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings;
        _serviceProvider = serviceProvider;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task StartConsumingAsync(string topic, Func<BaseEvent, Task> messageHandler, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Kafka consumer for topic: {Topic}", topic);
        
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = $"inventory-service-{topic}",
            ClientId = Environment.MachineName,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 6000,
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
        };

        using var consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
            {
                _logger.LogError("Kafka Consumer Error: {ErrorReason}", error.Reason);
            })
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                _logger.LogInformation("Assigned partitions: [{Partitions}]", string.Join(", ", partitions));
            })
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                _logger.LogInformation("Revoked partitions: [{Partitions}]", string.Join(", ", partitions));
            })
            .Build();

        consumer.Subscribe(topic);
        _logger.LogInformation("Started consuming from topic: {Topic}", topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(1000));
                    
                    if (consumeResult != null)
                    {
                        _logger.LogInformation("Received message from topic {Topic}: Key={Key}, Value={Value}", 
                            consumeResult.Topic, consumeResult.Message.Key, consumeResult.Message.Value);
                        
                        // For order-created topic, deserialize directly to OrderCreatedEvent
                        if (consumeResult.Topic == "order-created")
                        {
                            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(consumeResult.Message.Value, _jsonOptions);
                            if (orderEvent != null)
                            {
                                await messageHandler(orderEvent);
                                consumer.Commit(consumeResult);
                                _logger.LogDebug("OrderCreatedEvent processed and committed for topic {Topic}", consumeResult.Topic);
                            }
                        }
                        else
                        {
                            // For other topics, try to determine the event type first
                            var eventTypeDocument = JsonDocument.Parse(consumeResult.Message.Value);
                            if (eventTypeDocument.RootElement.TryGetProperty("eventType", out var eventTypeElement))
                            {
                                var eventType = eventTypeElement.GetString();
                                BaseEvent? specificEvent = eventType switch
                                {
                                    "OrderCreatedEvent" => JsonSerializer.Deserialize<OrderCreatedEvent>(consumeResult.Message.Value, _jsonOptions),
                                    "InventoryReservedEvent" => JsonSerializer.Deserialize<InventoryReservedEvent>(consumeResult.Message.Value, _jsonOptions),
                                    _ => null
                                };

                                if (specificEvent != null)
                                {
                                    await messageHandler(specificEvent);
                                    consumer.Commit(consumeResult);
                                    _logger.LogDebug("Message processed and committed for topic {Topic}", consumeResult.Topic);
                                }
                            }
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Consume error: {ErrorReason}", ex.Error.Reason);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "JSON deserialization error: {Message}", ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Message handling error: {Message}", ex.Message);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer operation was cancelled for topic: {Topic}", topic);
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Consumer closed for topic: {Topic}", topic);
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }
}
