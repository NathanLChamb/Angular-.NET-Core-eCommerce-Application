using eCommerce.Domain.Catalog.Options;

namespace eCommerce.Application.Interfaces.Repositories
{
    public interface IOptionRepository
    {
        Task<Option?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<Option>> GetByIdsAsync(List<int> ids, CancellationToken ct);
        Task AddAsync(Option option, CancellationToken ct);
        void Remove(Option option);
    }
}
