using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property<int>("OrderId")
                .IsRequired();

            builder.Property(i => i.ProductVariantId)
                .IsRequired();

            builder.HasIndex("OrderId", nameof(OrderItem.ProductVariantId));

            builder.OwnsOne(i => i.Product, product =>
            {
                product.Property(p => p.ProductName)
                    .HasColumnName("ProductName")
                    .HasMaxLength(200)
                    .IsRequired();

                product.Property(p => p.Sku)
                    .HasColumnName("Sku")
                    .HasMaxLength(64)
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

            builder.OwnsOne(i => i.UnitPrice, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("UnitPrice")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.Property(i => i.Quantity)
                .IsRequired();

            builder.Ignore(i => i.LineTotal);

            builder.HasOne<ProductVariant>()
                .WithMany()
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
