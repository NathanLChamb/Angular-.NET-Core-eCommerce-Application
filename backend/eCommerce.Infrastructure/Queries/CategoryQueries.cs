using eCommerce.Application.Features.Categories.DTOs;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Shared;
using eCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Queries
{
    public class CategoryQueries(
        eCommerceContext context) 
        : ICategoryQueries
    {
        private readonly eCommerceContext _context = context;

        public async Task<PagedResult<CategoryResponse>> GetPagedAsync(
            PaginationParams pageParams, 
            CancellationToken ct)
        {
            var query = _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Id);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((pageParams.PageNumber - 1) * pageParams.PageSize)
                .Take(pageParams.PageSize)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(ct);

            return new PagedResult<CategoryResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageParams.PageNumber,
                PageSize = pageParams.PageSize
            };
        }
        public async Task<CategoryResponse?> GetByIdAsync(
            int id, 
            CancellationToken ct)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
