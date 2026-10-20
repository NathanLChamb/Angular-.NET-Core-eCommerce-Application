using eCommerce.Application.Features.Orders.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Orders.Commands.CancelOrder
{
    public record CancelOrderCommand(
        int OrderId,
        string UserId
    ) : IRequest<OrderResponse>;
}
