namespace KafkaMicroservices.Shared.Infrastructure.Interfaces;

/// <summary>
/// Generic interface for message publishing to external systems
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publishes a message to the specified topic
    /// </summary>
    Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a message to the specified topic with a key
    /// </summary>
    Task PublishAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes multiple messages to the specified topic
    /// </summary>
    Task PublishManyAsync<T>(string topic, IEnumerable<T> messages, CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic interface for message consumption from external systems
/// </summary>
public interface IMessageConsumer
{
    /// <summary>
    /// Starts consuming messages from the specified topic
    /// </summary>
    Task StartConsumingAsync<T>(string topic, Func<T, Task> messageHandler, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts consuming messages from multiple topics
    /// </summary>
    Task StartConsumingAsync<T>(IEnumerable<string> topics, Func<string, T, Task> messageHandler, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops consuming messages
    /// </summary>
    Task StopConsumingAsync();
}

/// <summary>
/// Interface for message serialization/deserialization
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Serializes an object to string
    /// </summary>
    string Serialize<T>(T obj);

    /// <summary>
    /// Deserializes a string to the specified type
    /// </summary>
    T? Deserialize<T>(string data);

    /// <summary>
    /// Deserializes a string to the specified type
    /// </summary>
    object? Deserialize(string data, Type type);
}

/// <summary>
/// Interface for message broker configuration
/// </summary>
public interface IMessageBrokerSettings
{
    string ConnectionString { get; }
    string ClientId { get; }
    TimeSpan MessageTimeout { get; }
    int RetryCount { get; }
    Dictionary<string, object> AdditionalSettings { get; }
}