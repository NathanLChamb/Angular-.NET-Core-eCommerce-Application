using eCommerce.Domain.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(pv => pv.Id);
            
            builder.HasIndex(pv => pv.Sku)
                .IsUnique();
            builder.HasIndex(pv => new
            {
                pv.ProductId,
                pv.Sku
            });

            builder.Property(pv => pv.Sku)
                .IsRequired()
                .HasMaxLength(64);
            
            builder.Property(pv => pv.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(pv => pv.StockQuantity)
                .IsRequired();

            builder.Property(pv => pv.CreatedAt)
                .IsRequired();

            builder.Property(pv => pv.UpdatedAt)
                .IsRequired(false);

            builder.HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(pv => pv.ProductVariantOptionValues)
                .WithOne(pvov => pvov.ProductVariant)
                .HasForeignKey(pvov => pvov.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Version)
                .IsRowVersion();
        }
    }
}
