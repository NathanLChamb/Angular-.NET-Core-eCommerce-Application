using eCommerce.Application.Features.Carts.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Carts.Commands.RemoveCartItem
{
    public record RemoveCartItemCommand(
        int ProductVariantId
    ) : IRequest<CartResponse>;
}
