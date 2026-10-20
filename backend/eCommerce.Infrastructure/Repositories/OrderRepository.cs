using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Order;
using eCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class OrderRepository(
        eCommerceContext context) 
        : IOrderRepository
    {
        private readonly eCommerceContext _context = context;

        public async Task AddAsync(
            Order order, 
            CancellationToken ct)
        {
            await _context.Orders.AddAsync(order, ct);
        }

        public async Task<Order?> GetByIdAsync(
            int id, 
            CancellationToken ct)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        public async Task<Order?> GetByIdAsync(
            int id,
            string? userId,
            Guid? anonymousId,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userId) && anonymousId is null)
                return null;

            if (!string.IsNullOrWhiteSpace(userId) && anonymousId is not null)
                throw new ArgumentException(
                    "An order cannot be identified by both userId and anonymousId.");

            var query = _context.Orders
                .Include(o => o.Items)
                .Where(o => o.Id == id);

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(o => o.UserId == userId);
            else
                query = query.Where(o => o.AnonymousId == anonymousId);

            return await query.SingleOrDefaultAsync(ct);
        }
    }
}
