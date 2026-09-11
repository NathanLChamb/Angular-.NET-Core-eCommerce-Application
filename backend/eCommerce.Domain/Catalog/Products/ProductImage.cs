namespace eCommerce.Domain.Catalog.Products
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Url { get; set; } = null!;
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public Product Product { get; set; } = null!;
    }
}
