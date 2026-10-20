using eCommerce.Domain.Carts;

namespace eCommerce.Application.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetAsync(
            string? userId,
            Guid? anonymousId,
            CancellationToken ct);

        Task<Cart?> GetByIdAsync(
            int cartId,
            CancellationToken ct);

        Task AddAsync(
            Cart cart,
            CancellationToken ct);

        void Remove(Cart cart);
    }
}
