using eCommerce.Domain.Order;

namespace eCommerce.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(
            Order order,
            CancellationToken ct);

        Task<Order?> GetByIdAsync(
            int id,
            CancellationToken ct);

        Task<Order?> GetByIdAsync(
            int id,
            string? userId,
            Guid? anonymousId,
            CancellationToken ct);
    }
}
