using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Carts;
using eCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class CartRepository(
        eCommerceContext context) 
        : ICartRepository
    {
        private readonly eCommerceContext _context = context;

        public async Task<Cart?> GetAsync(string? userId, Guid? anonymousId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId) && anonymousId is null)
                throw new ArgumentException(
                    "Either userId or anonymousId must be provided.");

            if (!string.IsNullOrWhiteSpace(userId) && anonymousId is not null)
                throw new ArgumentException(
                    "A cart cannot be identified by both userId and anonymousId.");

            return await _context.Carts
                .Include(c => c.Items)
                .SingleOrDefaultAsync(
                    c =>
                        (!string.IsNullOrWhiteSpace(userId) && c.UserId == userId) ||
                        (anonymousId != null && c.AnonymousId == anonymousId),
                    ct);
        }

        public async Task<Cart?> GetByIdAsync(int cartId, CancellationToken ct)
        {
            return await _context.Carts
               .Include(c => c.Items)
               .SingleOrDefaultAsync(c => c.Id == cartId, ct);
        }
        public async Task AddAsync(Cart cart, CancellationToken ct)
        {
            await _context.Carts.AddAsync(cart, ct);
        }

        public void Remove(Cart cart)
        {
            _context.Carts.Remove(cart);
        }
    }
}
