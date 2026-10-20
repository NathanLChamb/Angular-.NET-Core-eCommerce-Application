namespace eCommerce.Application.Features.Products.DTOs
{
    public class CreateProductRequest
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public List<int> CategoryIds { get; set; } = [];
        public List<int> OptionIds { get; set; } = [];
        public List<CreateProductVariantRequest> ProductVariants { get; set; } = [];
    }
}
