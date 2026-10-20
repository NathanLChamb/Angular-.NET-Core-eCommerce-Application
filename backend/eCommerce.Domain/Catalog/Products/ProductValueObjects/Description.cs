namespace eCommerce.Domain.Catalog.Products.ProductValueObjects
{
    public record Description
    {
        public string Value { get; }
        public Description(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(
                "Product description cannot be empty", nameof(value));
            if (value.Length > 300) throw new ArgumentOutOfRangeException(
                nameof(value), "Product description must be 300 characters or less");

            Value = value.Trim();
        }
        private Description()
        {
            Value = string.Empty;
        }
        public static Description None { get; } = new Description();
    }
}