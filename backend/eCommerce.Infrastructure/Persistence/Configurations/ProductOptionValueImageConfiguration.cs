using eCommerce.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class ProductOptionValueImageConfiguration : IEntityTypeConfiguration<ProductOptionValueImage>
    {
        public void Configure(EntityTypeBuilder<ProductOptionValueImage> builder)
        {
            builder.HasKey(povi => povi.Id);

            builder.Property(povi => povi.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(povi => povi.DisplayOrder)
                .IsRequired();

            builder.Property(povi => povi.IsDefault)
                .IsRequired();

            builder.HasOne(povi => povi.Product)
                .WithMany(p => p.ProductOptionValueImages)
                .HasForeignKey(povi => povi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(povi => povi.OptionValues)
                .WithOne(poviov => poviov.ProductOptionValueImage)
                .HasForeignKey(poviov => poviov.OptionValueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
