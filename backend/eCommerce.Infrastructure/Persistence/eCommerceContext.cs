using Microsoft.EntityFrameworkCore;
using eCommerce.Domain.Metadata;
using eCommerce.Application.Interfaces;
using eCommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using eCommerce.Domain.Order;
using eCommerce.Domain.Catalog.Products;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.Categories;
using eCommerce.Domain.Carts;

namespace eCommerce.Infrastructure.Persistence
{
    public class eCommerceContext(
        DbContextOptions<eCommerceContext> options)
        : IdentityDbContext<ApplicationUser>(options), 
        IeCommerceContext, 
        IUnitOfWork
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(eCommerceContext).Assembly);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<OptionValue> OptionValues { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CatalogAttribute> CatalogAttributes { get; set; }
        public DbSet<ProductCatalogAttribute> ProductCatalogAttributes { get; set; }
    }
}