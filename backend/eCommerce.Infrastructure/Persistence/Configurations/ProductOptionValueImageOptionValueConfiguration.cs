using eCommerce.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class ProductOptionValueImageOptionValueConfiguration : IEntityTypeConfiguration<ProductOptionValueImageOptionValue>
    {
        public void Configure(EntityTypeBuilder<ProductOptionValueImageOptionValue> builder)
        {
            builder.HasKey(poviov => new
            {
                poviov.ProductOptionValueImageId,
                poviov.OptionValueId
            });

            builder.HasOne(poviov => poviov.ProductOptionValueImage)
                .WithMany(povi => povi.OptionValues)
                .HasForeignKey(poviov => poviov.ProductOptionValueImageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(poviov => poviov.OptionValue)
                .WithMany(ov => ov.ProductOptionValues)
                .HasForeignKey(poviov => poviov.OptionValueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
