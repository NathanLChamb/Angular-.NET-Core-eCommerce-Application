using eCommerce.Domain.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.OwnsOne(o => o.OrderNumber, orderNumber =>
            {
                orderNumber.Property(on => on.Value)
                    .HasColumnName("OrderNumber")
                    .HasMaxLength(32)
                    .IsRequired();

                orderNumber.HasIndex(on => on.Value)
                    .IsUnique();
            });

            builder.Property(o => o.UserId)
                .HasMaxLength(450);

            builder.Property(o => o.AnonymousId);

            builder.OwnsOne(o => o.ShippingAddress, shippingAddress =>
            {
                shippingAddress.Property(sa => sa.Value)
                .HasMaxLength(500)
                .IsRequired();
            });

            builder.Property(o => o.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(o => o.OrderDate)
                .IsRequired();

            builder.Property(o => o.UpdatedAt);

            builder.Ignore(o => o.TotalPrice);

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(o => o.Items)
                .HasField("_items");

            builder.HasIndex(o => o.UserId);

            builder.HasIndex(o => o.AnonymousId);
        }
    }
}
