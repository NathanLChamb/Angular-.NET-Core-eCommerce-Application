using eCommerce.Application.Features.Cart.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Cart.Commands.AddCartItem
{
    public record AddCartItemCommand(
        string UserId,
        int ProductVariantId,
        int Quantity
    ) : IRequest<CartResponse>;
}
