using eCommerce.Domain.Carts.ValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ValueObjects;

namespace eCommerce.Domain.Order.ValueObjects
{
    public sealed record OrderItemData(
        int ProductVariantId,
        ProductSnapshot Product,
        Price UnitPrice,
        int Quantity);
}
