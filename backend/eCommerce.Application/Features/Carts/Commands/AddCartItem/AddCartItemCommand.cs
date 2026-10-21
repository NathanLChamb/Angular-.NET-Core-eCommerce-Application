using eCommerce.Application.Features.Carts.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Carts.Commands.AddCartItem
{
    public record AddCartItemCommand(
        int ProductVariantId,
        int Quantity
    ) : IRequest<CartResponse>;
}
