namespace KafkaMicroservices.Shared.Infrastructure.Interfaces;

/// <summary>
/// Interface for database context abstraction
/// </summary>
public interface IDbContext
{
    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the entity set for the specified type
    /// </summary>
    IQueryable<T> Set<T>() where T : class;

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for database migrations and initialization
/// </summary>
public interface IDatabaseInitializer
{
    /// <summary>
    /// Ensures the database is created and up to date
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds the database with initial data
    /// </summary>
    Task SeedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Migrates the database to the latest version
    /// </summary>
    Task MigrateAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for health checking capabilities
/// </summary>
public interface IHealthCheck
{
    /// <summary>
    /// Performs a health check
    /// </summary>
    Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Health check result
/// </summary>
public class HealthCheckResult
{
    public bool IsHealthy { get; set; }
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public Exception? Exception { get; set; }

    public static HealthCheckResult Healthy(string description = "Healthy", Dictionary<string, object>? data = null)
        => new() { IsHealthy = true, Description = description, Data = data ?? new() };

    public static HealthCheckResult Unhealthy(string description = "Unhealthy", Exception? exception = null, Dictionary<string, object>? data = null)
        => new() { IsHealthy = false, Description = description, Exception = exception, Data = data ?? new() };
}