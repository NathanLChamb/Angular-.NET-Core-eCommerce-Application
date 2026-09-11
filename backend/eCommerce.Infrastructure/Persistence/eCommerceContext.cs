using Microsoft.EntityFrameworkCore;
using eCommerce.Domain.Metadata;
using eCommerce.Application.Interfaces;
using eCommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using eCommerce.Domain.Cart;
using eCommerce.Domain.Order;
using eCommerce.Domain.Catalog.Products;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.Categories;

namespace eCommerce.Infrastructure.Persistence
{
    public class eCommerceContext : IdentityDbContext<ApplicationUser>, IeCommerceContext, IUnitOfWork
    {
        public eCommerceContext(DbContextOptions<eCommerceContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(eCommerceContext).Assembly);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<OptionValue> OptionValues { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CatalogAttribute> CatalogAttributes { get; set; }
        public DbSet<ProductCatalogAttribute> ProductCatalogAttributes { get; set; }
        public DbSet<ProductOptionValueImage> ProductOptionValueImages { get; set; }
        public DbSet<ProductOptionValueImageOptionValue> ProductOptionValueImageOptionValues { get; set; }
    }
}