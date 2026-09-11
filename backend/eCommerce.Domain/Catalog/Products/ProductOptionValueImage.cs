namespace eCommerce.Domain.Catalog.Products
{
    public class ProductOptionValueImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public bool IsDefault { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public List<ProductOptionValueImageOptionValue> OptionValues { get; set; } = new();
    }
}
