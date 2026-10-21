using eCommerce.Application.Features.Carts.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Carts.Commands.ClearCart
{
    public record ClearCartCommand(
    ) : IRequest<CartResponse>;
}
