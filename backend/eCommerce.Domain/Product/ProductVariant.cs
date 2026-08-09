namespace eCommerce.Domain.Product
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public uint Version { get; set; }
        public required string Sku { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public List<ProductVariantOptionValue> ProductVariantOptionValues { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
