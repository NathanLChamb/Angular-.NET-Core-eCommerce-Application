namespace eCommerce.Domain.Catalog.Products.ProductValueObjects
{
    public record Name
    {
        public string Value { get; }
        public Name(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(
                "Product name cannot be empty", nameof(value));

            Value = value.Trim();
        }
    }
}
