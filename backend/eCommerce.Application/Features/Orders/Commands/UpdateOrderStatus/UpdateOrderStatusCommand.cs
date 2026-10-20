using eCommerce.Application.Features.Orders.DTOs;
using eCommerce.Domain.Order;
using MediatR;

namespace eCommerce.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(
        int OrderId,
        OrderStatus Status
    ) : IRequest<OrderResponse>;
}