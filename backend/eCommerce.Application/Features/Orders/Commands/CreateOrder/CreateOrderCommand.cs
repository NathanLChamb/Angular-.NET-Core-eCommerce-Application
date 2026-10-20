using eCommerce.Application.Features.Orders.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(
        string UserId,
        string ShippingAddress
    ) : IRequest<OrderResponse>;
}
