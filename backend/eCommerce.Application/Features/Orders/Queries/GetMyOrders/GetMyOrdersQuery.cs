using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Features.Orders.Filters;
using MediatR;

namespace eCommerce.Application.Features.Orders.Queries.GetMyOrders
{
    public record GetMyOrdersQuery(
        string UserId,
        OrderSearchFilter Filter
    ) : IRequest<IReadOnlyList<OrderResponse>>;
}
