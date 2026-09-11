//using eCommerce.Domain.Catalog.Products;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace eCommerce.Infrastructure.Persistence.Configurations
//{
//    public class ProductOptionConfiguration : IEntityTypeConfiguration<ProductOption>
//    {
//        public void Configure(EntityTypeBuilder<ProductOption> builder)
//        {
//            builder.HasKey(po => new
//            {
//                po.ProductId,
//                po.OptionId
//            });

//            builder.HasOne(po => po.Product)
//                .WithMany(p => p.ProductOptions)
//                .HasForeignKey(po => po.ProductId)
//                .OnDelete(DeleteBehavior.Cascade);

//            builder.HasOne(po => po.Option)
//                .WithMany(o => o.ProductOptions)
//                .HasForeignKey(po => po.OptionId)
//                .OnDelete(DeleteBehavior.Cascade);
//        }
//    }
//}
