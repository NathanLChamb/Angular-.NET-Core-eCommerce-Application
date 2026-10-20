using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Application.Features.Orders.Filters;
using MediatR;

namespace eCommerce.Application.Features.Orders.Queries.GetAllOrders
{
    public record GetAllOrdersQuery(
        OrderSearchFilter Filter
    ) : IRequest<IReadOnlyList<OrderResponse>>;
}
