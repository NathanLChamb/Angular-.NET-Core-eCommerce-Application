using eCommerce.Application.Features.Cart.DTOs;

namespace eCommerce.Application.Interfaces.Queries
{
    public interface ICartQueries
    {
        Task<ReadCartDto?> GetByIdAsync(int cartId, CancellationToken ct);
    }
}
