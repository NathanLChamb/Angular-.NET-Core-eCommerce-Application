using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Features.Options.DTOs;

namespace eCommerce.Application.Features.Products.DTOs
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public List<CategoryResponse> Categories { get; set; } = [];
        public List<OptionResponse> Options { get; set; } = [];
        public List<ProductVariantResponse> ProductVariants { get; set; } = [];
    }
}
