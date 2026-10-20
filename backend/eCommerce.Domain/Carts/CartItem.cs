using eCommerce.Domain.Carts.ValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ValueObjects;

namespace eCommerce.Domain.Carts
{
    public class CartItem
    {
        public int Id { get; private set; }
        public int ProductVariantId { get; private set; }
        public ProductSnapshot Product { get; private set; }
        public int Quantity { get; private set; }
        public Price UnitPrice { get; private set; }
        public decimal TotalPrice => UnitPrice.Amount * Quantity;

        public CartItem(int productVariantId, ProductSnapshot product, int quantity, Price unitPrice)
        {
            if (productVariantId <= 0)
                throw new ArgumentException(
                    "Product variant ID must be greater than zero.",
                    nameof(productVariantId));

            if (quantity <= 0)
                throw new ArgumentException(
                    "Quantity must be greater than zero.",
                    nameof(quantity));

            ProductVariantId = productVariantId;
            Product = product;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
        internal CartItem() { }

        public void IncreaseQuantity(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException(
                "Quantity must be greater than zero.", nameof(quantity));

            Quantity += quantity;
        }
        public void DecreaseQuantity(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException(
                "Quantity must be greater than zero.", nameof(quantity));

            if (quantity >= Quantity)
                throw new ArgumentException(
                    "Cannot decrease quantity to or below zero.",
                    nameof(quantity));

            Quantity -= quantity;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException(
                "Quantity must be greater than zero.", nameof(quantity));

            Quantity = quantity;
        }
    }
}
