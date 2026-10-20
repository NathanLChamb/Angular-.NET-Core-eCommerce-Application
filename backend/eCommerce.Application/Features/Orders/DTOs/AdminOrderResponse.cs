namespace eCommerce.Application.Features.Orders.DTOs
{
    public class AdminOrderResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<OrderItemResponse> Items { get; set; } = [];
    }
}
