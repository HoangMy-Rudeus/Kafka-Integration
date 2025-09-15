namespace KafkaMicroservices.NotificationService.Services;

public interface INotificationService
{
    Task SendOrderConfirmationAsync(string customerId, Guid orderId, decimal totalAmount);
    Task SendInventoryReservationNotificationAsync(string customerId, Guid orderId);
    Task<IEnumerable<Notification>> GetNotificationsAsync(string customerId);
}

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly List<Notification> _notifications = new(); // In-memory storage for demo

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(string customerId, Guid orderId, decimal totalAmount)
    {
        var message = $"Your order {orderId} has been confirmed. Total amount: ${totalAmount:F2}";
        
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Message = message,
            Type = "OrderConfirmation",
            Timestamp = DateTime.UtcNow,
            IsRead = false
        };

        _notifications.Add(notification);
        
        _logger.LogInformation("Order confirmation notification sent to customer {CustomerId} for order {OrderId}", 
            customerId, orderId);

        // Simulate sending email/SMS
        await SimulateSendingNotification(notification);
    }

    public async Task SendInventoryReservationNotificationAsync(string customerId, Guid orderId)
    {
        var message = $"Inventory has been reserved for your order {orderId}. Your order is being processed.";
        
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Message = message,
            Type = "InventoryReservation",
            Timestamp = DateTime.UtcNow,
            IsRead = false
        };

        _notifications.Add(notification);
        
        _logger.LogInformation("Inventory reservation notification sent to customer {CustomerId} for order {OrderId}", 
            customerId, orderId);

        await SimulateSendingNotification(notification);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsAsync(string customerId)
    {
        var customerNotifications = _notifications
            .Where(n => n.CustomerId == customerId)
            .OrderByDescending(n => n.Timestamp);
            
        return await Task.FromResult(customerNotifications);
    }

    private async Task SimulateSendingNotification(Notification notification)
    {
        // Simulate async notification sending (email, SMS, push notification, etc.)
        await Task.Delay(100);
        _logger.LogInformation("Notification sent: {Type} to {CustomerId}", notification.Type, notification.CustomerId);
    }
}

public class Notification
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
}
