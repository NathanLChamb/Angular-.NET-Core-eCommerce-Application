using eCommerce.Application.Features.Options.DTOs;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Shared;
using eCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Queries
{
    public class OptionQueries(
        eCommerceContext context) 
        : IOptionQueries
    {
        private readonly eCommerceContext _context = context;

        public async Task<PagedResult<OptionResponse>> GetPagedAsync(
            PaginationParams pageParams, 
            CancellationToken ct)
        {
            var query = _context.Options
                .AsNoTracking()
                .OrderBy(o => o.Id);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((pageParams.PageNumber - 1) * pageParams.PageSize)
                .Take(pageParams.PageSize)
                .Select(o => new OptionResponse
                {
                    Id = o.Id,
                    Name = o.Name,
                    OptionValues = o.Values.Select(ov => new OptionValueResponse
                    {
                        Id = ov.Id,
                        Value = ov.Value
                    }).ToList()
                }).ToListAsync(ct);

            return new PagedResult<OptionResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageParams.PageNumber,
                PageSize = pageParams.PageSize
            };
        }

        public async Task<OptionResponse?> GetByIdAsync(
            int id, 
            CancellationToken ct)
        {
            return await _context.Options
                .AsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new OptionResponse
                {
                    Id = o.Id,
                    Name = o.Name,
                    OptionValues = o.Values.Select(ov => new OptionValueResponse
                    {
                        Id = ov.Id,
                        Value = ov.Value
                    }).ToList()
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
