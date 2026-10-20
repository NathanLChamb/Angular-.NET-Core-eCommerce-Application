using eCommerce.Application.Features.Options.DTOs;

namespace eCommerce.Application.Features.Products.DTOs
{
    public class ProductVariantResponse
    {
        public int Id { get; set; }
        public required string Sku { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<OptionValueResponse> OptionValues { get; set; } = [];
        public List<PictureResponse> Pictures { get; set; } = [];
    }
}
