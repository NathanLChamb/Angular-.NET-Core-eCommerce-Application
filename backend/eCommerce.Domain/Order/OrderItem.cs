using eCommerce.Domain.Carts.ValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ValueObjects;

namespace eCommerce.Domain.Order
{
    public class OrderItem
    {
        public int Id { get; private set; }
        public int ProductVariantId { get; private set; }
        public ProductSnapshot Product { get; private set; } = null!;
        public Price UnitPrice { get; private set; } = null!;
        public int Quantity { get; private set; }
        public decimal LineTotal => UnitPrice.Amount * Quantity;
        private OrderItem() { }

        internal OrderItem(int productVariantId, ProductSnapshot product, Price unitPrice, int quantity)
        {
            if (productVariantId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(productVariantId),
                    "Product variant ID must be greater than zero.");
            }

            ArgumentNullException.ThrowIfNull(product);
            ArgumentNullException.ThrowIfNull(unitPrice);

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "Quantity must be greater than zero.");
            }

            ProductVariantId = productVariantId;
            Product = product;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}
