using eCommerce.Domain.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId)
                .HasMaxLength(450);

            builder.Property(c => c.AnonymousId);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt);

            builder.HasMany(c => c.Items)
                .WithOne()
                .HasForeignKey("CartId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.UserId)
                .IsUnique()
                .HasFilter("\"UserId\" IS NOT NULL");

            builder.HasIndex(c => c.AnonymousId)
                .IsUnique()
                .HasFilter("\"AnonymousId\" IS NOT NULL");
        }
    }
}
