using eCommerce.Application.Features.Carts.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using MediatR;

namespace eCommerce.Application.Features.Carts.Queries.GetCart
{
    public class GetCartHandler(
        ICartQueries queries, 
        ICurrentUser user) 
        : IRequestHandler<GetCartQuery, CartResponse>
    {
        private readonly ICurrentUser _user = user;
        private readonly ICartQueries _queries = queries;

        public async Task<CartResponse> Handle(
            GetCartQuery request, 
            CancellationToken ct)
        {
            var cart = await _queries.GetCartDetailsAsync(
                _user.UserId, 
                _user.AnonymousId, 
                ct);

            return cart ?? new CartResponse
            {
                Id = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                Items = [],
                Subtotal = 0m
            };
        }
    }
}
