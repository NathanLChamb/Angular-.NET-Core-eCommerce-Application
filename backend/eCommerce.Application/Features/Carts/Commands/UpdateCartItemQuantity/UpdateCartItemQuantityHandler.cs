using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Carts.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using MediatR;

namespace eCommerce.Application.Features.Carts.Commands.UpdateCartItemQuantity
{
    public class UpdateCartItemQuantityHandler(
        ICartRepository carts, 
        ICartQueries queries, 
        ICurrentUser user, 
        IUnitOfWork unitOfWork) 
        : IRequestHandler<UpdateCartItemQuantityCommand, CartResponse>
    {
        private readonly ICartRepository _carts = carts;
        private readonly ICartQueries _queries = queries;
        private readonly ICurrentUser _user = user;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<CartResponse> Handle(
            UpdateCartItemQuantityCommand request,
            CancellationToken ct)
        {
            var cart = await _carts.GetAsync(
                _user.UserId, _user.AnonymousId, ct)
                ?? throw new NotFoundException(
                    "Cart not found from provided ID");

            cart.UpdateItem(request.ProductVariantId, request.Quantity);

            await _unitOfWork.SaveChangesAsync(ct);

            return await _queries.GetCartDetailsAsync(_user.UserId, _user.AnonymousId, ct)
                ?? throw new InvalidOperationException(
                    "Cart not found");
        }
    }
}
