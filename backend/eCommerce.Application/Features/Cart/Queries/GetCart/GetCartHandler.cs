using eCommerce.Application.Features.Cart.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Features.Cart.Queries.GetCart
{
    public class GetCartHandler : IRequestHandler<GetCartQuery, ReadCartDto>
    {
        private readonly IeCommerceContext _context;
        private readonly ICartQueries _queries;

        public GetCartHandler(IeCommerceContext context, ICartQueries queries)
        {
            _context = context;
            _queries = queries;
        }

        public async Task<ReadCartDto> Handle(GetCartQuery request, CancellationToken ct)
        {
            var cartId = await _context.Carts
            .Where(c => c.UserId == request.UserId)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(ct);

            if (cartId.HasValue)
            {
                var cart = await _queries.GetByIdAsync(
                    cartId.Value,
                    ct);

                if (cart != null)
                {
                    return cart;
                }
            }

            return new ReadCartDto
            {
                Items = new(),
                TotalPrice = 0
            };
        }
    }
}
