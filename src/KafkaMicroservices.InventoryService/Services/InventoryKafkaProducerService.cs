using KafkaMicroservices.Shared.Configuration;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Services;
using Confluent.Kafka;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KafkaMicroservices.InventoryService.Services;

public class InventoryKafkaProducerService : IKafkaProducer<BaseEvent>, IDisposable
{
    private readonly ILogger<InventoryKafkaProducerService> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IProducer<string, string> _producer;
    private readonly JsonSerializerOptions _jsonOptions;

    public InventoryKafkaProducerService(ILogger<InventoryKafkaProducerService> logger, KafkaSettings kafkaSettings)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings;

        var config = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            ClientId = Environment.MachineName,
            Acks = Acks.Leader,
            MessageTimeoutMs = 30000,
            CompressionType = CompressionType.Snappy
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
            {
                _logger.LogError("Kafka Producer Error: {ErrorReason}", error.Reason);
            })
            .Build();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            IncludeFields = true
        };
    }

    public async Task ProduceAsync(string topic, BaseEvent message, CancellationToken cancellationToken = default)
    {
        try
        {
            // Serialize using the actual runtime type instead of BaseEvent to include all properties
            var messageJson = JsonSerializer.Serialize(message, message.GetType(), _jsonOptions);
            _logger.LogInformation("Producing message to topic {Topic}: {Message}", topic, messageJson);
            
            var kafkaMessage = new Message<string, string>
            {
                Key = message.EventId.ToString(),
                Value = messageJson,
                Timestamp = new Timestamp(message.Timestamp)
            };

            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
            _logger.LogInformation("Message delivered to topic {Topic}, partition {Partition}, offset {Offset}", 
                deliveryResult.Topic, deliveryResult.Partition, deliveryResult.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to deliver message to topic {Topic}: {ErrorReason}", topic, ex.Error.Reason);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing message to topic {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}