namespace eCommerce.Application.Features.Products.DTOs
{
    public class ProductVariantOptionValueResponse
    {
        public int Id { get; set; }
        public required string Value { get; set; }
        public int OptionId { get; set; }
    }
}
