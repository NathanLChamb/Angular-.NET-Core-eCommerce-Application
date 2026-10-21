using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Carts.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Application.Interfaces.Repositories;
using eCommerce.Domain.Carts.ValueObjects;
using eCommerce.Domain.Catalog.ProductVariants.ValueObjects;
using MediatR;

namespace eCommerce.Application.Features.Carts.Commands.AddCartItem
{
    public class AddCartItemHandler(
        ICartRepository carts, 
        ICartQueries queries, 
        IProductQueries productQueries, 
        ICurrentUser user, 
        IUnitOfWork unitOfWork) 
        : IRequestHandler<AddCartItemCommand, CartResponse>
    {
        private readonly ICartRepository _carts = carts;
        private readonly ICartQueries _queries = queries;
        private readonly IProductQueries _productQueries = productQueries;
        private readonly ICurrentUser _user = user;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<CartResponse> Handle(AddCartItemCommand request, CancellationToken ct)
        {
            var product = await _productQueries.GetVariantDetailsAsync(request.ProductVariantId, ct)
                ?? throw new NotFoundException(
                    "Product variant not found.");

            var cart = await _carts.GetAsync(_user.UserId, _user.AnonymousId, ct);
            if (cart == null)
            {
                
                cart = new Domain.Carts.Cart(_user.UserId, _user.AnonymousId);
                await _carts.AddAsync(cart, ct);
            }

            var snapshot = new ProductSnapshot(product.ProductName, product.Sku, product.ImageUrl);

            cart.AddItem(request.ProductVariantId, snapshot, request.Quantity, new Price(product.UnitPrice));

            await _unitOfWork.SaveChangesAsync(ct);

            return await _queries.GetCartDetailsAsync(_user.UserId, _user.AnonymousId, ct)
                ?? throw new InvalidOperationException("Cart not found");
        }
    }
}
