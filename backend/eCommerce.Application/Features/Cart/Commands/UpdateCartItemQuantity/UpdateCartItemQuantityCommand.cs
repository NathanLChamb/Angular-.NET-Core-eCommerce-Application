using eCommerce.Application.Features.Cart.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Cart.Commands.UpdateCartItemQuantity
{
    public record UpdateCartItemQuantityCommand(
        int ProductVariantId,
        int Quantity
    ) : IRequest<CartResponse>;
}
