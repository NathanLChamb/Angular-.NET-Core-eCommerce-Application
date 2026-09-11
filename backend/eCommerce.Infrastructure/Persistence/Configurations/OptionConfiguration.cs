using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class OptionConfiguration : IEntityTypeConfiguration<Option>
    {
        public void Configure(EntityTypeBuilder<Option> builder)
        { 
            builder.HasKey(o => o.Id);

            builder.HasIndex(o => o.Name)
                .IsUnique();

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasMany(o => o.Values)
                .WithOne()
                .HasForeignKey("OptionId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(o => o.Values)
                .HasField("_values");
        }
    }
}
