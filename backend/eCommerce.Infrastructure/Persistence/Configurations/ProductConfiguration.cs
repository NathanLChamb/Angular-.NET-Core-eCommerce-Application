using eCommerce.Domain.Catalog.Categories;
using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.OwnsOne(p => p.Name, name =>
            {
                name.Property(n => n.Value)
                    .HasColumnName("Name")
                    .IsRequired();
            });

            builder.OwnsOne(p => p.Description, description =>
            {
                description.Property(d => d.Value)
                    .HasColumnName("Description")
                    .IsRequired(false);
            });

            builder.HasMany(p => p.Categories)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>("ProductCategories",
                    category => category
                        .HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade),
                    product => product
                        .HasOne<Product>()
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.HasKey("ProductId", "CategoryId");
                    });
            builder.Navigation(p => p.Categories)
                .HasField("_categories");

            builder.HasMany(p => p.Options)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>("ProductOptions",
                    option => option
                        .HasOne<Option>()
                        .WithMany()
                        .HasForeignKey("OptionId")
                        .OnDelete(DeleteBehavior.Cascade),
                    product => product
                        .HasOne<Product>()
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.HasKey("ProductId", "OptionId");
                    });
            builder.Navigation(p => p.Options)
                .HasField("_options");

            builder.HasMany(p => p.Variants)
                .WithOne()
                .HasForeignKey("ProductId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(p => p.Variants)
                .HasField("_variants");
        }
    }
}
