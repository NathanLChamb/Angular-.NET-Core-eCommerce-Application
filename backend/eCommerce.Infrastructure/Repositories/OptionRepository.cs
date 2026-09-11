using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Options;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Persistence.Repositories
{
    public class OptionRepository : IOptionRepository
    {
        private readonly eCommerceContext _context;
        public OptionRepository(eCommerceContext context)
        {
            _context = context;
        }
        public async Task<Option?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Options
                .Include(o => o.Values)
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        public async Task<List<Option>> GetByIdsAsync(List<int> ids, CancellationToken ct)
        {
            var optionIds = ids.Distinct().ToList();

            return await _context.Options
                .Include(o => o.Values)
                .Where(o => optionIds.Contains(o.Id))
                .ToListAsync(ct);
        }

        public async Task AddAsync(Option option, CancellationToken ct)
        {
            await _context.Options.AddAsync(option, ct);
        }

        public void Remove(Option option)
        {
            _context.Options.Remove(option);
        }
    }
}
