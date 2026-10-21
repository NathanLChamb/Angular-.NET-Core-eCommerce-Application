namespace eCommerce.Application.Features.Carts.DTOs
{
    public class CartItemResponse
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public string? ImageUrl { get; set; }
    }
}
