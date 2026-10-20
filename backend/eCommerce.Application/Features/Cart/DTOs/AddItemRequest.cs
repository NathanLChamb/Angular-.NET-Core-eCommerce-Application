namespace eCommerce.Application.Features.Cart.DTOs
{
    public class AddItemRequest
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
