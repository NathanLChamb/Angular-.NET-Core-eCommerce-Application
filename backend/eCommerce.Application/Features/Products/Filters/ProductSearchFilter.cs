namespace eCommerce.Application.Features.Products.Filters
{
    public class ProductSearchFilter
    {
        public string? Search { get; set; }
        public List<int> CategoryIds { get; set; } = [];
        public List<int> OptionIds { get; set; } = [];
        public List<int> OptionValueIds { get; set; } = [];
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public ProductSort Sort { get; set; } = ProductSort.Default;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
