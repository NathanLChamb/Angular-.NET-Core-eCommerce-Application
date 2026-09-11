using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.ProductVariants;
using eCommerce.Domain.Catalog.ProductVariants.ProductVariantValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(pv => pv.Id);

            builder.HasIndex("ProductId", "Sku")
                .IsUnique();

            builder.HasIndex(pv => pv.Sku)
                .IsUnique();

            builder.Property(x => x.Version)
                .IsRowVersion();

            builder.Property(pv => pv.Sku)
                .HasConversion(
                    sku => sku.Value,
                    value => new ProductVariantSku(value))
                .HasColumnName("Sku")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(pv => pv.StockQuantity)
                .HasConversion(
                    stock => stock.Value,
                    value => new StockQuantity(value))
                .HasColumnName("StockQuantity")
                .IsRequired();

            builder.OwnsOne(pv => pv.Price, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Price")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.HasMany(pv => pv.OptionValues)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>("ProductVariantOptionValues",
                    optionValue => optionValue
                        .HasOne<OptionValue>()
                        .WithMany()
                        .HasForeignKey("OptionValueId")
                        .OnDelete(DeleteBehavior.Cascade),
                    variant => variant
                        .HasOne<ProductVariant>()
                        .WithMany()
                        .HasForeignKey("ProductVariantId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.HasKey("ProductVariantId", "OptionValueId");
                    });
        }
    }
}
