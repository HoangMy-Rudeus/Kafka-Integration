using KafkaMicroservices.NotificationService.Data;
using KafkaMicroservices.Shared.Models;
using Microsoft.EntityFrameworkCore;

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
    private readonly NotificationDbContext _context;

    public NotificationService(ILogger<NotificationService> logger, NotificationDbContext context)
    {
        _logger = logger;
        _context = context;
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
            Timestamp = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        
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
            Timestamp = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Inventory reservation notification sent to customer {CustomerId} for order {OrderId}", 
            customerId, orderId);

        // Simulate sending notification
        await SimulateSendingNotification(notification);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsAsync(string customerId)
    {
        return await _context.Notifications
            .Where(n => n.CustomerId == customerId)
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    private async Task SimulateSendingNotification(Notification notification)
    {
        // Simulate external service call (email, SMS, push notification)
        await Task.Delay(100); // Simulate network call
        _logger.LogInformation("Notification sent: {Message}", notification.Message);
    }
}
