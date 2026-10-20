using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Cart.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using MediatR;

namespace eCommerce.Application.Features.Cart.Commands.RemoveCartItem
{
    public class RemoveCartItemHandler(
        ICartRepository carts, 
        ICurrentUser user, 
        IUnitOfWork unitOfWork, 
        ICartQueries queries) 
        : IRequestHandler<RemoveCartItemCommand, CartResponse>
    {
        private readonly ICartRepository _carts = carts;
        private readonly ICurrentUser _user = user;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICartQueries _queries = queries;

        public async Task<CartResponse> Handle(
            RemoveCartItemCommand request,
            CancellationToken ct)
        {
            var cart = await _carts.GetAsync(
                _user.UserId, 
                _user.AnonymousId, 
                ct)
                ?? throw new NotFoundException(
                    "Cart not found from provided ID");

            cart.RemoveItem(request.ProductVariantId);

            await _unitOfWork.SaveChangesAsync(ct);

            return await _queries.GetCartDetailsAsync(
                _user.UserId, 
                _user.AnonymousId, 
                ct)
                ?? throw new InvalidOperationException(
                    "Cart not found");
        }
    }
}
