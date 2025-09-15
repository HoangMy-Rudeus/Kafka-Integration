using System.Text.Json;

namespace KafkaMicroservices.Shared.Services;

public interface IKafkaProducer<T>
{
    Task ProduceAsync(string topic, T message, CancellationToken cancellationToken = default);
}

public interface IKafkaConsumer<T>
{
    Task StartConsumingAsync(string topic, Func<T, Task> messageHandler, CancellationToken cancellationToken = default);
    void Dispose();
}

public static class MessageSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, Options);
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }
}
