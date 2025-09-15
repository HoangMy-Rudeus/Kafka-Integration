using KafkaMicroservices.NotificationService.Services;
using Microsoft.AspNetCore.Mvc;

namespace KafkaMicroservices.NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetNotifications(string customerId)
    {
        try
        {
            var notifications = await _notificationService.GetNotificationsAsync(customerId);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for customer {CustomerId}", customerId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("test-order-confirmation")]
    public async Task<IActionResult> TestOrderConfirmation([FromBody] TestOrderConfirmationRequest request)
    {
        try
        {
            await _notificationService.SendOrderConfirmationAsync(
                request.CustomerId,
                request.OrderId,
                request.TotalAmount);
            
            return Ok("Order confirmation notification sent");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending test order confirmation");
            return StatusCode(500, "Internal server error");
        }
    }
}

public class TestOrderConfirmationRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
}
