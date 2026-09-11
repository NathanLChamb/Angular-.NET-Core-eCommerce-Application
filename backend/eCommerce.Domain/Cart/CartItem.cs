using eCommerce.Domain.Catalog.ProductVariants;

namespace eCommerce.Domain.Cart
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
