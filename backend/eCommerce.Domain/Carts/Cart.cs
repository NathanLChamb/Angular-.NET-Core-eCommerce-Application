using eCommerce.Domain.Carts.ValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ValueObjects;

namespace eCommerce.Domain.Carts
{
    public class Cart
    {
        private readonly List<CartItem> _items = [];
        public int Id { get; private set; }
        public string? UserId { get; private set; }
        public Guid? AnonymousId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public IReadOnlyList<CartItem> Items => _items;
        public Cart(string? userId = null, Guid? anonymousId = null)
        {
            if (string.IsNullOrWhiteSpace(userId) && anonymousId is null)
            {
                throw new ArgumentException(
                    "A cart must have either a user ID or anonymous ID.");
            }

            if (!string.IsNullOrWhiteSpace(userId) && anonymousId is not null)
            {
                throw new ArgumentException(
                    "A cart cannot have both a user ID and anonymous ID.");
            }

            UserId = userId;
            AnonymousId = anonymousId;
        }
        internal Cart() { }

        public void AddItem(int productVariantId, ProductSnapshot product, int quantity, Price unitPrice)
        {
            if (productVariantId <= 0)
                throw new ArgumentException(
                    "Product variant ID must be greater than zero.",
                    nameof(productVariantId));

            ArgumentNullException.ThrowIfNull(product);

            if (quantity <= 0)
                throw new ArgumentException(
                    "Quantity must be greater than zero.",
                    nameof(quantity));

            var existingItem = _items.FirstOrDefault(
                i => i.ProductVariantId == productVariantId);

            if (existingItem is not null)
            {
                existingItem.IncreaseQuantity(quantity);
            }
            else
            {
                _items.Add(new CartItem(productVariantId, product, quantity, unitPrice));
            }

            Touch();
        }

        public void UpdateItem(int productVariantId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException(
                    "Quantity must be greater than zero.",
                    nameof(quantity));

            var item = FindItem(productVariantId);

            item.SetQuantity(quantity);

            Touch();
        }

        public void RemoveItem(int productVariantId)
        {
            var item = _items.FirstOrDefault(
                i => i.ProductVariantId == productVariantId);

            if (item is null)
                return;

            _items.Remove(item);

            Touch();
        }

        public void Clear()
        {
            if (_items.Count == 0)
                return;

            _items.Clear();

            Touch();
        }

        public void AssignToUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException(
                    "User ID is required.",
                    nameof(userId));

            UserId = userId;
            AnonymousId = null;

            Touch();
        }

        private CartItem FindItem(int productVariantId)
        {
            var item = _items.FirstOrDefault(i => i.ProductVariantId == productVariantId)
                ?? throw new ArgumentException(
                    $"Product variant {productVariantId} is not in the cart.",
                    nameof(productVariantId));

            return item;
        }

        private void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
