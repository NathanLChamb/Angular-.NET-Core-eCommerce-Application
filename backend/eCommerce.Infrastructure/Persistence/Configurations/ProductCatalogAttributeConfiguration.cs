using eCommerce.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class ProductCatalogAttributeConfiguration : IEntityTypeConfiguration<ProductCatalogAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductCatalogAttribute> builder)
        {
            builder.HasKey(pca => new
            {
                pca.ProductId,
                pca.CatalogAttributeId
            });

            builder.HasOne(pca => pca.Product)
                .WithMany(p => p.ProductCatalogAttributes)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pca => pca.CatalogAttribute)
                .WithMany(c => c.ProductCatalogAttributes)
                .HasForeignKey(pc => pc.CatalogAttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
