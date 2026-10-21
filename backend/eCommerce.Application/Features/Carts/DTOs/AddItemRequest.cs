namespace eCommerce.Application.Features.Carts.DTOs
{
    public class AddItemRequest
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
