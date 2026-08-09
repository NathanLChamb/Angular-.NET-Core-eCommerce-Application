namespace eCommerce.Application.Features.Orders.Filters
{
    public class OrderSearchFilter
    {
        public OrderSort Status { get; set; } = OrderSort.All;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
