using eCommerce.Application.Features.Orders.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Orders.Queries.GetOrderById
{
    public record GetOrderByIdQuery(
        int OrderId,
        string UserId
    ) : IRequest<OrderResponse?>;
}
