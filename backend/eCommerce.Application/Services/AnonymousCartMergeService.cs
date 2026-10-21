using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Repositories;

namespace eCommerce.Application.Services
{
    public sealed class AnonymousCartMergeService(
    ICartRepository carts,
    IUnitOfWork unitOfWork)
    : IAnonymousCartMergeService
    {
        public async Task MergeAsync(
            string userId,
            Guid? anonymousId,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException(
                    "User ID is required.", nameof(userId));

            if (anonymousId is null)
                return;

            var anonymousCart = await carts.GetAsync(
                null, anonymousId, ct);

            if (anonymousCart is null)
                return;

            var userCart = await carts.GetAsync(
                userId, null, ct);

            if (userCart is null)
            {
                anonymousCart.AssignToUser(userId);
            }
            else
            {
                userCart.MergeFrom(anonymousCart);
                carts.Remove(anonymousCart);
            }

            await unitOfWork.SaveChangesAsync(ct);
        }
    }
}
