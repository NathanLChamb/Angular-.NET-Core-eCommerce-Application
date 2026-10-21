namespace eCommerce.Application.Interfaces
{
    public interface IAnonymousCartMergeService
    {
        Task MergeAsync(
            string userId,
            Guid? anonymousId,
            CancellationToken ct);
    }
}
