namespace KafkaMicroservices.Shared.Events;

public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<OrderItemEvent> Items { get; init; } = new();
}

public record OrderItemEvent
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
}

public record InventoryUpdatedEvent
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int QuantityChanged { get; init; }
    public int CurrentStock { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record NotificationEvent
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Recipient { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public Dictionary<string, object> Metadata { get; init; } = new();
}
