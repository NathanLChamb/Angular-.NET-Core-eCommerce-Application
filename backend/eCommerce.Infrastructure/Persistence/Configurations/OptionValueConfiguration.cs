using eCommerce.Domain.Catalog.Options;
using eCommerce.Domain.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Persistence.Configurations
{
    public class OptionValueConfiguration : IEntityTypeConfiguration<OptionValue>
    {
        public void Configure(EntityTypeBuilder<OptionValue> builder)
        {
            builder.HasKey(ov => ov.Id);

            builder.Property(ov => ov.Value)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
