namespace eCommerce.Application.Features.Products.DTOs
{
    public class VariantDetailsResponse
    {
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
