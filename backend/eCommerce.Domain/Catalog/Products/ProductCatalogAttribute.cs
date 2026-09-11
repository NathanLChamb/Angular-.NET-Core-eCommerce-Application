using eCommerce.Domain.Metadata;

namespace eCommerce.Domain.Catalog.Products
{
    public class ProductCatalogAttribute
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int CatalogAttributeId { get; set; }
        public CatalogAttribute CatalogAttribute { get; set; } = null!;
    }
}
