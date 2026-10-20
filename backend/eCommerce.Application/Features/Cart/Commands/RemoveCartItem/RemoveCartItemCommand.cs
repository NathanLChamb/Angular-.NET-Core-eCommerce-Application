using eCommerce.Application.Features.Cart.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Cart.Commands.RemoveCartItem
{
    public record RemoveCartItemCommand(
        string UserId,
        int ProductVariantId
    ) : IRequest<CartResponse>;
}
