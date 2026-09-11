namespace eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects
{
    public record ProductVariantSku
    {
        public string Value { get; }
        public ProductVariantSku(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(
                "Sku cannot be empty", nameof(value));

            Value = value.Trim().ToUpperInvariant();
        }

        public override string ToString() => Value;
    }
}
