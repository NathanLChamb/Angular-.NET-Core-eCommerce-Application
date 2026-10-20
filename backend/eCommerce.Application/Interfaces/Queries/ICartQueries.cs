using eCommerce.Application.Features.Cart.DTOs;

namespace eCommerce.Application.Interfaces.Queries
{
    public interface ICartQueries
    {
        Task<CartResponse?> GetCartDetailsAsync(
            string? userId,
            Guid? anonymousId,
            CancellationToken ct);
    }
}
