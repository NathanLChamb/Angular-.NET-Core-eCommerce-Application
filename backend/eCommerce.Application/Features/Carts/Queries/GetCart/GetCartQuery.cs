using eCommerce.Application.Features.Carts.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Carts.Queries.GetCart
{
    public record GetCartQuery(
    ) : IRequest<CartResponse>;
}
