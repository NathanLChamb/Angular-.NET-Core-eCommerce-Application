namespace eCommerce.Application.Features.Products.DTOs
{
    public class UpdateProductRequest
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public List<int> CategoryIds { get; set; } = [];
        public List<int> OptionIds { get; set; } = [];
        public List<UpdateProductVariantRequest> ProductVariants { get; set; } = [];
    }
}
