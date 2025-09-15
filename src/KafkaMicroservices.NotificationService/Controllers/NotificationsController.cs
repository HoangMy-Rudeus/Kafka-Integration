using KafkaMicroservices.NotificationService.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KafkaMicroservices.NotificationService.Controllers;

/// <summary>
/// API for managing notifications in the microservices demo
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all notifications for a customer
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <returns>List of customer notifications</returns>
    /// <response code="200">Notifications retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{customerId}")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetNotifications([Required] string customerId)
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

    /// <summary>
    /// Test endpoint to send an order confirmation notification
    /// </summary>
    /// <param name="request">Order confirmation request</param>
    /// <returns>Success message</returns>
    /// <response code="200">Notification sent successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("test-order-confirmation")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

/// <summary>
/// Request model for testing order confirmation notifications
/// </summary>
public class TestOrderConfirmationRequest
{
    /// <summary>
    /// Customer ID to send notification to
    /// </summary>
    [Required]
    public string CustomerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Order ID for the confirmation
    /// </summary>
    [Required]
    public Guid OrderId { get; set; }
    
    /// <summary>
    /// Total amount of the order
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }
}
