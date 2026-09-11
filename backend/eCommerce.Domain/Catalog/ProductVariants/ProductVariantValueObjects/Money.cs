namespace eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects
{
    public record Money
    {
        public decimal Amount { get; }
        public Money(decimal amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(
                nameof(amount), "Amount must not be negative");
            if (decimal.Round(amount, 2) != amount) throw new ArgumentOutOfRangeException(
                nameof(amount), "Amount cannot have more than 2 decimals");

            Amount = amount;
        }
    }
}
