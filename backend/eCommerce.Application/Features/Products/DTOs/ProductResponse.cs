using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Features.Products.Images.DTOs;

namespace eCommerce.Application.Features.Products.DTOs
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public List<CategoryResponse> Categories { get; set; } = new();
        public List<ProductOptionResponse> Options { get; set; } = new();
        public List<ProductVariantResponse> ProductVariants { get; set; } = new();
        public List<ProductImageDto> ProductImages { get; set; } = new();
        public List<ProductOptionValueImageDto> ProductOptionValueImages { get; set; } = new();
    }
}
