using eCommerce.Domain.Order.ValueObjects;

namespace eCommerce.Domain.Order
{
    public class Order
    {
        private readonly List<OrderItem> _items = [];
        public int Id { get; private set; }
        public OrderNumber OrderNumber { get; private set; } = null!;
        public string? UserId { get; private set; }
        public Guid? AnonymousId { get; private set; }
        public ShippingAddress ShippingAddress { get; private set; } = null!;
        public OrderStatus Status { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public decimal TotalPrice => _items.Sum(i => i.LineTotal);

        private Order() { }

        public static Order Create(string? userId, Guid? anonymousId,
            OrderNumber orderNumber, ShippingAddress shippingAddress, IEnumerable<OrderItemData> items)
        {
            if (string.IsNullOrWhiteSpace(userId) == (anonymousId is null))
                throw new ArgumentException(
                    "An order must belong to either a user or an anonymous session.");

            ArgumentNullException.ThrowIfNull(items);

            var itemData = items.ToList();
            if (itemData.Count == 0) throw new InvalidOperationException(
                "Cannot create an order without items.");
            if (itemData.GroupBy(i => i.ProductVariantId).Any(g => g.Count() > 1))
                throw new InvalidOperationException(
                    "An order cannot contain duplicate product variants.");

            var order = new Order
            {
                UserId = string.IsNullOrWhiteSpace(userId)
                    ? null
                    : userId.Trim(),
                AnonymousId = anonymousId,
                OrderNumber = orderNumber,
                ShippingAddress = shippingAddress,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow
            };

            foreach (var item in itemData)
            {
                order._items.Add(new OrderItem(item.ProductVariantId, item.Product, item.UnitPrice, item.Quantity));
            }

            return order;
        }

        public void Process()
        {
            EnsureStatus(OrderStatus.Pending);

            Status = OrderStatus.Processing;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Ship()
        {
            EnsureStatus(OrderStatus.Processing);

            Status = OrderStatus.Shipped;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deliver()
        {
            EnsureStatus(OrderStatus.Shipped);

            Status = OrderStatus.Delivered;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status != OrderStatus.Pending &&
                Status != OrderStatus.Processing)
            {
                throw new InvalidOperationException(
                    "Only pending or processing orders can be cancelled.");
            }

            Status = OrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        private void EnsureStatus(OrderStatus expected)
        {
            if (Status != expected)
            {
                throw new InvalidOperationException(
                    $"Order must be {expected} to perform this operation.");
            }
        }
    }
}
