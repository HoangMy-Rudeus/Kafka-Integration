using Confluent.Kafka;
using System.Text.Json;

namespace KafkaMicroservices.Shared.Services;

public interface IKafkaConsumer
{
    Task StartConsumingAsync<T>(string topic, string groupId, Func<T, Task> handler, CancellationToken cancellationToken = default);
}

public class KafkaConsumer : IKafkaConsumer
{
    private readonly string _bootstrapServers;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaConsumer(string bootstrapServers)
    {
        _bootstrapServers = bootstrapServers;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task StartConsumingAsync<T>(string topic, string groupId, Func<T, Task> handler, CancellationToken cancellationToken = default)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = groupId,
            ClientId = Environment.MachineName,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 6000,
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
        };

        using var consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
            {
                Console.WriteLine($"Kafka Consumer Error: {error.Reason}");
            })
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
            })
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                Console.WriteLine($"Revoked partitions: [{string.Join(", ", partitions)}]");
            })
            .Build();

        consumer.Subscribe(topic);
        Console.WriteLine($"Started consuming from topic: {topic} with group: {groupId}");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(1000));
                    
                    if (consumeResult != null)
                    {
                        Console.WriteLine($"Received message: Key={consumeResult.Message.Key}, Value={consumeResult.Message.Value}");
                        
                        var message = JsonSerializer.Deserialize<T>(consumeResult.Message.Value, _jsonOptions);
                        if (message != null)
                        {
                            await handler(message);
                            consumer.Commit(consumeResult);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Consume error: {ex.Error.Reason}");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON deserialization error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Message handling error: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Consumer operation was cancelled");
        }
        finally
        {
            consumer.Close();
            Console.WriteLine("Consumer closed");
        }
    }
}
