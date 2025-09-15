using Confluent.Kafka;
using System.Text.Json;

namespace KafkaMicroservices.Shared.Services;

public interface IKafkaProducer
{
    Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default);
    Task ProduceAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default);
}

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaProducer(string bootstrapServers)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            ClientId = Environment.MachineName,
            Acks = Acks.Leader,
            MessageTimeoutMs = 30000,
            CompressionType = CompressionType.Snappy
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
            {
                Console.WriteLine($"Kafka Producer Error: {error.Reason}");
            })
            .Build();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
    {
        await ProduceAsync(topic, Guid.NewGuid().ToString(), message, cancellationToken);
    }

    public async Task ProduceAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default)
    {
        var messageValue = JsonSerializer.Serialize(message, _jsonOptions);
        
        var kafkaMessage = new Message<string, string>
        {
            Key = key,
            Value = messageValue,
            Timestamp = new Timestamp(DateTime.UtcNow)
        };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);
            Console.WriteLine($"Message delivered to topic {deliveryResult.Topic}, partition {deliveryResult.Partition}, offset {deliveryResult.Offset}");
        }
        catch (ProduceException<string, string> ex)
        {
            Console.WriteLine($"Failed to deliver message: {ex.Error.Reason}");
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}
