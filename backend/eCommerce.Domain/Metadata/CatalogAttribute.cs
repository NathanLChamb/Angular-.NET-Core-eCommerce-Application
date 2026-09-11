using eCommerce.Domain.Catalog.Products;

namespace eCommerce.Domain.Metadata
{
    public class CatalogAttribute
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
        public List<ProductCatalogAttribute> ProductCatalogAttributes { get; set; } = new();
    }
}
