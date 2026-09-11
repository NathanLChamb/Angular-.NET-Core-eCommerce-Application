using eCommerce.Application.Features.Options.DTOs;
using eCommerce.Application.Shared;

namespace eCommerce.Application.Interfaces.Queries
{
    public interface IOptionQueries
    {
        Task<PagedResult<OptionResponse>> GetPagedAsync(PaginationParams pageParams, CancellationToken ct);
        Task<OptionResponse?> GetByIdAsync(int id, CancellationToken ct);
    }
}
