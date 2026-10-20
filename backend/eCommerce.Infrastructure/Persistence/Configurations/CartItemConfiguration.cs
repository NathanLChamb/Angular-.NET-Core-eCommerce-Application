using eCommerce.Domain.Carts;
using eCommerce.Domain.Catalog.ProductVariants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property<int>("CartId");
            builder.HasIndex("CartId", nameof(CartItem.ProductVariantId))
                .IsUnique();

            builder.Property(i => i.ProductVariantId)
                .IsRequired();

            builder.OwnsOne(i => i.Product, product =>
            {
                product.Property(p => p.ProductName)
                    .HasColumnName("ProductName")
                    .IsRequired();

                product.Property(p => p.Sku)
                    .HasColumnName("Sku")
                    .IsRequired();

                product.Property(p => p.Image)
                    .HasColumnName("ProductImage")
                    .HasConversion(
                        image => image == null
                            ? null
                            : image.ToString(),
                        value => value == null
                            ? null
                            : new Uri(value, UriKind.Absolute));
            });

            builder.Property(i => i.Quantity)
                .IsRequired();

            builder.OwnsOne(pv => pv.UnitPrice, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("UnitPrice")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.Ignore(i => i.TotalPrice);

            builder.HasOne<ProductVariant>()
                .WithMany()
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
