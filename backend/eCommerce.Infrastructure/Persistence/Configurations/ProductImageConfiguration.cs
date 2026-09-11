using eCommerce.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(pi => pi.DisplayOrder)
                .IsRequired();

            builder.Property(pi => pi.CreatedAt)
                .IsRequired();

            builder.HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}