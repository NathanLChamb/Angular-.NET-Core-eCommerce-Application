using eCommerce.Domain.Cart;
using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.Products;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Metadata;
using eCommerce.Domain.Order;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Interfaces
{
    public interface IeCommerceContext
    {
        DbSet<Product> Products { get; }
        DbSet<Option> Options { get; }
        DbSet<OptionValue> OptionValues { get; }
        DbSet<ProductVariant> ProductVariants { get; }
        DbSet<Cart> Carts { get; }
        DbSet<CartItem> CartItems { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<ProductImage> ProductImages { get; }
        DbSet<ProductOptionValueImage> ProductOptionValueImages { get; }
        DbSet<CatalogAttribute> CatalogAttributes { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
