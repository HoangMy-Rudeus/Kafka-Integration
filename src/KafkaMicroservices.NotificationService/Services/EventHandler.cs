using KafkaMicroservices.Shared.Configuration;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Services;

namespace KafkaMicroservices.NotificationService.Services;

public class EventHandler : BackgroundService
{
    private readonly ILogger<EventHandler> _logger;
    private readonly IKafkaConsumer<BaseEvent> _kafkaConsumer;
    private readonly IServiceProvider _serviceProvider;

    public EventHandler(
        ILogger<EventHandler> logger,
        IKafkaConsumer<BaseEvent> kafkaConsumer,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _kafkaConsumer = kafkaConsumer;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Event Handler started");
        
        // Start multiple consumers for different events
        var tasks = new List<Task>
        {
            Task.Run(() => _kafkaConsumer.StartConsumingAsync(Topics.OrderCreated, HandleOrderCreatedEvent, stoppingToken), stoppingToken),
            Task.Run(() => _kafkaConsumer.StartConsumingAsync(Topics.InventoryReserved, HandleInventoryReservedEvent, stoppingToken), stoppingToken)
        };

        await Task.WhenAny(tasks);
    }

    private async Task HandleOrderCreatedEvent(BaseEvent baseEvent)
    {
        try
        {
            if (baseEvent is OrderCreatedEvent orderCreatedEvent)
            {
                _logger.LogInformation("Processing OrderCreatedEvent for order {OrderId}", orderCreatedEvent.Order.Id);
                
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                
                await notificationService.SendOrderConfirmationAsync(
                    orderCreatedEvent.Order.CustomerId,
                    orderCreatedEvent.Order.Id,
                    orderCreatedEvent.Order.TotalAmount);
                
                _logger.LogInformation("Order confirmation notification processed for order {OrderId}", orderCreatedEvent.Order.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing OrderCreatedEvent");
        }
    }

    private async Task HandleInventoryReservedEvent(BaseEvent baseEvent)
    {
        try
        {
            if (baseEvent is InventoryReservedEvent inventoryReservedEvent)
            {
                _logger.LogInformation("Processing InventoryReservedEvent for order {OrderId}", inventoryReservedEvent.OrderId);
                
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                
                // Note: In a real scenario, we'd need to get customer ID from the order
                // For this demo, we'll use a placeholder
                await notificationService.SendInventoryReservationNotificationAsync(
                    "CUSTOMER_ID", // In real scenario, get from order service
                    inventoryReservedEvent.OrderId);
                
                _logger.LogInformation("Inventory reservation notification processed for order {OrderId}", inventoryReservedEvent.OrderId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing InventoryReservedEvent");
        }
    }

    public override void Dispose()
    {
        _kafkaConsumer?.Dispose();
        base.Dispose();
    }
}
