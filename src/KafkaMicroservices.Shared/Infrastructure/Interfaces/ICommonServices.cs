namespace KafkaMicroservices.Shared.Infrastructure.Interfaces;

/// <summary>
/// Interface for logging abstraction
/// </summary>
public interface IAppLogger<T>
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
    void LogDebug(string message, params object[] args);
    void LogCritical(string message, params object[] args);
    void LogCritical(Exception exception, string message, params object[] args);
}

/// <summary>
/// Interface for caching abstraction
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task ClearAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for external service communication
/// </summary>
public interface IHttpClientService
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for date/time abstraction for testing
/// </summary>
public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTimeOffset OffsetNow { get; }
    DateTimeOffset OffsetUtcNow { get; }
}

/// <summary>
/// Interface for generating unique identifiers
/// </summary>
public interface IIdGenerator
{
    Guid NewGuid();
    string NewId();
    string NewId(int length);
}