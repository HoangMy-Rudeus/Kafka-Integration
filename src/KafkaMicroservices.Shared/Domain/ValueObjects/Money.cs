namespace KafkaMicroservices.Shared.Domain.ValueObjects;

/// <summary>
/// Money value object representing monetary amounts
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    public static Money Zero => new Money(0);

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new ArgumentException("Money amount cannot be negative", nameof(amount));
        
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

        Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add different currencies: {Currency} and {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot subtract different currencies: {Currency} and {other.Currency}");

        var result = Amount - other.Amount;
        if (result < 0)
            throw new InvalidOperationException("Subtraction would result in negative amount");

        return new Money(result, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentException("Multiplier cannot be negative", nameof(multiplier));

        return new Money(Amount * multiplier, Currency);
    }

    public static Money operator +(Money left, Money right)
    {
        return left.Add(right);
    }

    public static Money operator -(Money left, Money right)
    {
        return left.Subtract(right);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return money.Multiply(multiplier);
    }

    public override string ToString()
    {
        return $"{Amount:C} {Currency}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}