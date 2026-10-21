namespace eCommerce.Application.Features.Carts.DTOs
{
    public class CartResponse
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CartItemResponse> Items { get; set; } = [];
        public decimal Subtotal { get; set; }
    }
}
