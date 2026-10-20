using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Products;
using eCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class ProductRepository(
        eCommerceContext context) 
        : IProductRepository
    {
        private readonly eCommerceContext _context = context;

        public async Task<Product?> GetByIdAsync(
            int id, 
            CancellationToken ct)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Options)
                    .ThenInclude(o => o.Values)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.OptionValues)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<IReadOnlyList<Product>> GetByVariantIdsAsync(
            IReadOnlyCollection<int> variantIds, 
            CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(variantIds);

            if (variantIds.Count == 0)
                return [];

            var ids = variantIds.Distinct().ToArray();

            return await _context.Products
                .Include(p => p.Variants)
                    .ThenInclude(v => v.OptionValues)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Pictures)
                .Where(p => p.Variants.Any(v => ids.Contains(v.Id)))
                .ToListAsync(ct);
        }

        public async Task AddAsync(
            Product product, 
            CancellationToken ct)
        {
            await _context.Products.AddAsync(product, ct);
        }

        public void Remove(
            Product product)
        {
            _context.Products.Remove(product);
        }
    }
}
