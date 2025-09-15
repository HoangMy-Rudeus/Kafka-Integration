using KafkaMicroservices.Shared.Domain.Entities;
using KafkaMicroservices.Shared.Domain.ValueObjects;
using KafkaMicroservices.Shared.Domain.Events;

namespace KafkaMicroservices.Shared.Domain.Entities;

/// <summary>
/// Order domain entity representing a customer order
/// </summary>
public class Order : BaseEntity
{
    public CustomerId CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // Required for EF Core
    private Order() : base() 
    {
        CustomerId = CustomerId.Empty;
        TotalAmount = Money.Zero;
        Status = OrderStatus.Pending;
    }

    public Order(CustomerId customerId, IEnumerable<OrderItem> items) : base()
    {
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
        TotalAmount = Money.Zero; // Initialize before validation
        
        if (items == null || !items.Any())
            throw new ArgumentException("Order must have at least one item", nameof(items));

        _items.AddRange(items);
        RecalculateTotal();
        Status = OrderStatus.Pending;

        // Add domain event for order creation
        AddDomainEvent(new OrderCreatedDomainEvent(Id, CustomerId, _items.ToList(), TotalAmount));
    }

    public void AddItem(OrderItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        
        _items.Add(item);
        RecalculateTotal();
        SetUpdatedAt();
    }

    public void RemoveItem(ProductId productId)
    {
        if (productId == null) throw new ArgumentNullException(nameof(productId));
        
        var item = _items.FirstOrDefault(i => i.ProductId.Equals(productId));
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotal();
            SetUpdatedAt();
        }
    }

    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm order in {Status} status");

        Status = OrderStatus.Confirmed;
        SetUpdatedAt();

        AddDomainEvent(new OrderConfirmedDomainEvent(Id, CustomerId));
    }

    public void CompleteOrder()
    {
        if (Status != OrderStatus.Confirmed && Status != OrderStatus.Processing)
            throw new InvalidOperationException($"Cannot complete order in {Status} status");

        Status = OrderStatus.Completed;
        SetUpdatedAt();

        AddDomainEvent(new OrderCompletedDomainEvent(Id, CustomerId));
    }

    public void CancelOrder(string reason)
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed order");

        Status = OrderStatus.Cancelled;
        SetUpdatedAt();

        AddDomainEvent(new OrderCancelledDomainEvent(Id, CustomerId, reason));
    }

    private void RecalculateTotal()
    {
        var total = _items.Sum(item => item.LineTotal.Amount);
        TotalAmount = new Money(total);
    }
}

/// <summary>
/// Order status enumeration
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Completed = 3,
    Cancelled = 4
}