using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly eCommerceContext _context;
        public ProductRepository(eCommerceContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Options)
                    .ThenInclude(o => o.Values)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.OptionValues)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task AddAsync(Product product, CancellationToken ct)
        {
            await _context.Products.AddAsync(product, ct);
        }

        public void Remove(Product product)
        {
            _context.Products.Remove(product);
        }
    }
}
