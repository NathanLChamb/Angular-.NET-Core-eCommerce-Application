using eCommerce.Application.Features.Cart.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Cart.Commands.UpdateCartItemQuantity
{
    public record UpdateCartItemQuantityCommand(
        string UserId,
        int ProductVariantId,
        int Quantity
    ) : IRequest<CartResponse>;
}
