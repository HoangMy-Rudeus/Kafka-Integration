namespace KafkaMicroservices.Shared.Domain.ValueObjects;

/// <summary>
/// Customer ID value object
/// </summary>
public class CustomerId : ValueObject
{
    public string Value { get; private set; }

    public static CustomerId Empty => new CustomerId(string.Empty);

    public CustomerId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Customer ID cannot be null or empty", nameof(value));

        Value = value.Trim();
    }

    public static implicit operator string(CustomerId customerId)
    {
        return customerId.Value;
    }

    public static explicit operator CustomerId(string value)
    {
        return new CustomerId(value);
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

/// <summary>
/// Product ID value object
/// </summary>
public class ProductId : ValueObject
{
    public string Value { get; private set; }

    public static ProductId Empty => new ProductId(string.Empty);

    public ProductId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Product ID cannot be null or empty", nameof(value));

        Value = value.Trim();
    }

    public static implicit operator string(ProductId productId)
    {
        return productId.Value;
    }

    public static explicit operator ProductId(string value)
    {
        return new ProductId(value);
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

/// <summary>
/// Product Name value object
/// </summary>
public class ProductName : ValueObject
{
    public string Value { get; private set; }

    public static ProductName Empty => new ProductName(string.Empty);

    public ProductName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Product name cannot be null or empty", nameof(value));

        Value = value.Trim();
    }

    public static implicit operator string(ProductName productName)
    {
        return productName.Value;
    }

    public static explicit operator ProductName(string value)
    {
        return new ProductName(value);
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

/// <summary>
/// Quantity value object
/// </summary>
public class Quantity : ValueObject
{
    public int Value { get; private set; }

    public static Quantity Zero => new Quantity(0);

    public Quantity(int value)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(value));

        Value = value;
    }

    public static implicit operator int(Quantity quantity)
    {
        return quantity.Value;
    }

    public static explicit operator Quantity(int value)
    {
        return new Quantity(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}