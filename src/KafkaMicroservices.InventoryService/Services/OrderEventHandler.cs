using KafkaMicroservices.Shared.Configuration;
using KafkaMicroservices.Shared.Events;
using KafkaMicroservices.Shared.Services;

namespace KafkaMicroservices.InventoryService.Services;

public class OrderEventHandler : BackgroundService
{
    private readonly ILogger<OrderEventHandler> _logger;
    private readonly IKafkaConsumer<BaseEvent> _kafkaConsumer;
    private readonly IServiceProvider _serviceProvider;

    public OrderEventHandler(
        ILogger<OrderEventHandler> logger,
        IKafkaConsumer<BaseEvent> kafkaConsumer,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _kafkaConsumer = kafkaConsumer;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order Event Handler started");
        
        await _kafkaConsumer.StartConsumingAsync(Topics.OrderCreated, HandleOrderCreatedEvent, stoppingToken);
    }

    private async Task HandleOrderCreatedEvent(BaseEvent baseEvent)
    {
        try
        {
            if (baseEvent is OrderCreatedEvent orderCreatedEvent)
            {
                _logger.LogInformation("Processing OrderCreatedEvent for order {OrderId}", orderCreatedEvent.Order.Id);
                
                using var scope = _serviceProvider.CreateScope();
                var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();
                
                var success = await inventoryService.ReserveInventoryAsync(
                    orderCreatedEvent.Order.Id, 
                    orderCreatedEvent.Order.Items);

                if (success)
                {
                    _logger.LogInformation("Inventory reserved successfully for order {OrderId}", orderCreatedEvent.Order.Id);
                    
                    // TODO: Publish InventoryReservedEvent to Kafka
                    var inventoryReservedEvent = new InventoryReservedEvent
                    {
                        OrderId = orderCreatedEvent.Order.Id,
                        ReservedItems = orderCreatedEvent.Order.Items.Select(item => new ReservedItem
                        {
                            ProductId = item.ProductId,
                            QuantityReserved = item.Quantity
                        }).ToList()
                    };
                    
                    _logger.LogInformation("Would publish InventoryReservedEvent for order {OrderId}", orderCreatedEvent.Order.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to reserve inventory for order {OrderId}", orderCreatedEvent.Order.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing OrderCreatedEvent");
        }
    }

    public override void Dispose()
    {
        _kafkaConsumer?.Dispose();
        base.Dispose();
    }
}
