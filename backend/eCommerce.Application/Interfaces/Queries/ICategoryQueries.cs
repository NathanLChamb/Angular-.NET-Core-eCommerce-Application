using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Shared;

namespace eCommerce.Application.Interfaces.Queries
{
    public interface ICategoryQueries
    {
        Task<PagedResult<CategoryResponse>> GetPagedAsync(PaginationParams pageParams, CancellationToken ct);
        Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken ct);
    }
}
