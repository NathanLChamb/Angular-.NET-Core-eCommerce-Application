namespace eCommerce.Domain.Catalog.Products.ProductValueObjects
{
    public record ProductName
    {
        public string Value { get; }
        public ProductName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(
                "Product name cannot be empty", nameof(value));

            Value = value.Trim();
        }
    }
}
