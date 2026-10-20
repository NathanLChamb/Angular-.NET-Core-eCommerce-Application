using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Domain.Order;

namespace eCommerce.Application.Features.Orders.Mappings
{
    public static class OrderProjection
    {
        public static IQueryable<OrderResponse> ToOrderDto(this IQueryable<Order> query)
        {
            return query.Select(o => new OrderResponse
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber.Value,
                ShippingAddress = o.ShippingAddress.Value,
                Status = o.Status.ToString(),
                OrderDate = o.OrderDate,
                TotalPrice = o.Items.Sum(
                        i => i.UnitPrice.Amount * i.Quantity),

                Items = o.Items.Select(i => new OrderItemResponse
                {
                    Id = i.Id,
                    ProductVariantId = i.ProductVariantId,
                    ProductName = i.Product.ProductName,
                    Sku = i.Product.Sku,
                    ImageUrl = i.Product.Image == null
                        ? null
                        : i.Product.Image.ToString(),
                    UnitPrice = i.UnitPrice.Amount,
                    Quantity = i.Quantity,
                    LineTotal = i.UnitPrice.Amount * i.Quantity
                }).ToList()
            });
        }
    }
}
