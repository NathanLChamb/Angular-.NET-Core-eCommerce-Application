namespace eCommerce.Application.Features.Products.DTOs
{
    public class UpdateProductVariantDto
    {
        public int? Id { get; set; }
        public required string Sku { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<int> OptionValueIds { get; set; } = new();
    }
}
