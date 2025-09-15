namespace KafkaMicroservices.Shared.Configuration;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string GroupId { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public bool EnableAutoCommit { get; set; } = true;
    public string AutoOffsetReset { get; set; } = "earliest";
    public int SessionTimeoutMs { get; set; } = 6000;
    public int MessageTimeoutMs { get; set; } = 30000;
}

public static class Topics
{
    public const string OrderCreated = "order-created";
    public const string OrderConfirmed = "order-confirmed";
    public const string InventoryReserved = "inventory-reserved";
    public const string NotificationSent = "notification-sent";
}
