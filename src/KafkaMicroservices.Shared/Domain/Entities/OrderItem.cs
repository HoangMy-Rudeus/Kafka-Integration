using KafkaMicroservices.Shared.Domain.ValueObjects;

namespace KafkaMicroservices.Shared.Domain.Entities;

/// <summary>
/// Order item representing a product in an order
/// </summary>
public class OrderItem
{
    public ProductId ProductId { get; private set; }
    public ProductName ProductName { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money LineTotal => new Money(UnitPrice.Amount * Quantity.Value);

    // Required for EF Core
    private OrderItem()
    {
        ProductId = ProductId.Empty;
        ProductName = ProductName.Empty;
        Quantity = Quantity.Zero;
        UnitPrice = Money.Zero;
    }

    public OrderItem(ProductId productId, ProductName productName, Quantity quantity, Money unitPrice)
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));

        if (quantity.Value <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        
        if (unitPrice.Amount < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));
    }

    public void UpdateQuantity(Quantity newQuantity)
    {
        if (newQuantity == null) throw new ArgumentNullException(nameof(newQuantity));
        if (newQuantity.Value <= 0) throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
    }

    public void UpdateUnitPrice(Money newUnitPrice)
    {
        if (newUnitPrice == null) throw new ArgumentNullException(nameof(newUnitPrice));
        if (newUnitPrice.Amount < 0) throw new ArgumentException("Unit price cannot be negative", nameof(newUnitPrice));

        UnitPrice = newUnitPrice;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not OrderItem other) return false;
        return ProductId.Equals(other.ProductId);
    }

    public override int GetHashCode()
    {
        return ProductId.GetHashCode();
    }
}