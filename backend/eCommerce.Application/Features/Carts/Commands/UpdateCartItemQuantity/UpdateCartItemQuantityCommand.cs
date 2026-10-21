using eCommerce.Application.Features.Carts.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Carts.Commands.UpdateCartItemQuantity
{
    public record UpdateCartItemQuantityCommand(
        int ProductVariantId,
        int Quantity
    ) : IRequest<CartResponse>;
}
