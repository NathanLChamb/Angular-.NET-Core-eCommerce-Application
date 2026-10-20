using eCommerce.Application.Features.Cart.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Cart.Queries.GetCart
{
    public record GetCartQuery(
        string UserId
    ) : IRequest<CartResponse>;
}
