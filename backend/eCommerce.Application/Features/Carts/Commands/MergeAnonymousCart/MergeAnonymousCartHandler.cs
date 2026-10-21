using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Carts.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Carts;
using MediatR;

namespace eCommerce.Application.Features.Carts.Commands.MergeAnonymousCart
{
    public class MergeAnonymousCartHandler(
        ICartRepository carts,
        ICartQueries queries,
        ICurrentUser currentUser,
        IAnonymousCartSession anonymousSession,
        IUnitOfWork unitOfWork)
        : IRequestHandler<MergeAnonymousCartCommand, CartResponse>
    {
        public async Task<CartResponse> Handle(
            MergeAnonymousCartCommand request,
            CancellationToken ct)
        {
            var userId = currentUser.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new BusinessRuleException(
                    "You must be authenticated to merge carts.");
            }

            var anonymousId = anonymousSession.GetAnonymousId();

            var userCart = await carts.GetAsync(
                userId, null, ct);

            if (anonymousId is null)
            {
                return await GetOrCreateUserCart(userId, userCart, ct);
            }

            var anonymousCart = await carts.GetAsync(
                null, anonymousId, ct);

            if (anonymousCart is null)
            {
                return await GetOrCreateUserCart(userId, userCart, ct);
            }

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

            anonymousSession.Clear();

            return await queries.GetCartDetailsAsync(
                userId, null, ct)
                ?? throw new InvalidOperationException(
                    "The merged cart could not be retrieved.");
        }

        private async Task<CartResponse> GetOrCreateUserCart(
            string userId,
            Cart? userCart,
            CancellationToken ct)
        {
            if (userCart is null)
            {
                userCart = new Cart(userId);
                await carts.AddAsync(userCart, ct);
                await unitOfWork.SaveChangesAsync(ct);
            }

            return await queries.GetCartDetailsAsync(
                userId, null, ct)
                ?? throw new InvalidOperationException(
                    "The user's cart could not be retrieved.");
        }
    }
}
