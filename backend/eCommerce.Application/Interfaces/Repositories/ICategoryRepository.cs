using eCommerce.Domain.Catalog.Categories;

namespace eCommerce.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<Category>> GetByIdsAsync(List<int> ids, CancellationToken ct);
        Task AddAsync(Category category, CancellationToken ct);
        void Remove(Category category);
    }
}
