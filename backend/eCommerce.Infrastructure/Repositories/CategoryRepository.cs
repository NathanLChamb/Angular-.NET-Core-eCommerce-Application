using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Categories;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly eCommerceContext _context;
        public CategoryRepository(eCommerceContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<List<Category>> GetByIdsAsync(List<int> ids, CancellationToken ct)
        {
            var categoryIds = ids.Distinct().ToList();

            return await _context.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync(ct);
        }

        public async Task AddAsync(Category category, CancellationToken ct)
        {
            await _context.Categories.AddAsync(category, ct);
        }
        public void Remove(Category category)
        {
            _context.Categories.Remove(category);
        }
    }
}
