using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Cart.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Features.Cart.Commands.AddCartItem
{
    public class AddCartItemHandler : IRequestHandler<AddCartItemCommand, ReadCartDto>
    {
        private readonly IeCommerceContext _context;
        private readonly ICartQueries _queries;
        public AddCartItemHandler(IeCommerceContext context, ICartQueries queries)
        {
            _context = context;
            _queries = queries;
        }

        public async Task<ReadCartDto> Handle(AddCartItemCommand request, CancellationToken ct)
        {
            if (request.Quantity <= 0)
            {
                throw new BusinessRuleException(
                    "Quantity must be greater than zero.");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(
                    c => c.UserId == request.UserId,
                    ct);

            if (cart == null)
            {
                cart = new Domain.Cart.Cart
                {
                    UserId = request.UserId
                };

                _context.Carts.Add(cart);
            }

            var productVariant = await _context.ProductVariants
                .FirstOrDefaultAsync(
                    pv => pv.Id == request.ProductVariantId,
                    ct);

            if (productVariant == null)
            {
                throw new NotFoundException(
                    "Product variant not found.");
            }

            var existingItem = cart.Items
                .FirstOrDefault(
                    i => i.ProductVariantId == request.ProductVariantId);

            var newQuantity = existingItem == null
                ? request.Quantity
                : existingItem.Quantity + request.Quantity;

            if (newQuantity > productVariant.StockQuantity.Value)
            {
                throw new BusinessRuleException(
                    "Insufficient stock.");
            }

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Items.Add(new Domain.Cart.CartItem
                {
                    ProductVariantId = productVariant.Id,
                    ProductVariant = productVariant,
                    Quantity = request.Quantity
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            var result = await _queries.GetByIdAsync(cart.Id, ct);

            if (result == null)
            {
                throw new NotFoundException(
                    "Cart could not be loaded after adding the item.");
            }

            return result;
        }
    }
}
