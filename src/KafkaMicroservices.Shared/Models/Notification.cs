namespace KafkaMicroservices.Shared.Models;

/// <summary>
/// Represents a customer notification
/// </summary>
public class Notification
{
    /// <summary>
    /// Unique notification identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Customer ID who should receive the notification
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of notification (e.g., "OrderConfirmation", "OrderShipped", etc.)
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Notification message content
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// When the notification was created
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
