using eCommerce.Application.Features.Cart.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Cart.Commands.ClearCart
{
    public record ClearCartCommand(
    ) : IRequest<CartResponse>;
}
