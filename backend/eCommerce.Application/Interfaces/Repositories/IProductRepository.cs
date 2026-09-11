using eCommerce.Domain.Catalog.Products;

namespace eCommerce.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id, CancellationToken ct);
        Task AddAsync(Product product, CancellationToken ct);
        void Remove(Product product);
    }
}
