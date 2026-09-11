using eCommerce.Domain.Catalog.ProductVariants;

namespace eCommerce.Domain.Order
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
        public required string ProductName { get; set; }
        public required string Sku { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public int Quantity { get; set; }
    }
}
