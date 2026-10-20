namespace eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects
{
    public record Sku
    {
        public string Value { get; }
        public Sku(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(
                "Sku cannot be empty", nameof(value));

            Value = value.Trim().ToUpperInvariant();
        }

        public override string ToString() => Value;
    }
}
