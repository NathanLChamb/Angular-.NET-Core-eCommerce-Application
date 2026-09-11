namespace eCommerce.Domain.Catalog.Products.ProductValueObjects
{
    public record ProductDescription
    {
        public string Value { get; }
        public ProductDescription(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(
                "Product description cannot be empty", nameof(value));
            if (value.Length > 300) throw new ArgumentOutOfRangeException(
                nameof(value), "Product description must be 300 characters or less");

            Value = value.Trim();
        }
        private ProductDescription()
        {
            Value = string.Empty;
        }
        public static ProductDescription None { get; } = new ProductDescription();
    }
}