namespace eCommerce.Domain.Carts.ValueObjects
{
    public record ProductSnapshot
    {
        public string ProductName { get; private set; }
        public string Sku { get; private set; }
        public Uri? Image { get; private set; }
        public ProductSnapshot(string productName, string sku, string? image = null)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException(
                    "Product name cannot be empty.",
                    nameof(productName));

            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException(
                    "SKU cannot be empty.",
                    nameof(sku));

            Uri? uri = null;

            if (!string.IsNullOrWhiteSpace(image))
            {
                if (!Uri.TryCreate(image, UriKind.Absolute, out uri))
                    throw new ArgumentException(
                        "Picture location must be a valid absolute URI.",
                        nameof(image));
            }

            ProductName = productName.Trim();
            Sku = sku.Trim();
            Image = uri;
        }
        internal ProductSnapshot() { }
    }
}