using eCommerce.Application.Features.Carts.DTOs;
using MediatR;

namespace eCommerce.Application.Features.Carts.Commands.MergeAnonymousCart
{
    public record MergeAnonymousCartCommand
    : IRequest<CartResponse>;
}
