using eCommerce.Application.Features.Products.DTOs;
using eCommerce.Application.Features.Products.Filters;
using eCommerce.Application.Shared;

namespace eCommerce.Application.Interfaces.Queries
{
    public interface IProductQueries
    {
        Task<PagedResult<ProductResponse>> GetPagedAsync(ProductSearchFilter filter, CancellationToken ct);
        Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct);
    }
}
