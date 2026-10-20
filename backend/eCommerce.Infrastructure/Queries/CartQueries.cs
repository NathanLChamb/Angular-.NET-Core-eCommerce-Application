using eCommerce.Application.Features.Cart.DTOs;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Queries
{
    public class CartQueries(
        eCommerceContext context) 
        : ICartQueries
    {
        private readonly eCommerceContext _context = context;

        public async Task<CartResponse?> GetCartDetailsAsync(
            string? userId,
            Guid? anonymousId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId) && anonymousId is null)
                throw new ArgumentException(
                    "Either userId or anonymousId must be provided.");

            if (!string.IsNullOrWhiteSpace(userId) && anonymousId is not null)
                throw new ArgumentException(
                    "A cart cannot be identified by both userId and anonymousId.");

            var cart = await _context.Carts
                .AsNoTracking()
                .Where(c =>
                    (!string.IsNullOrWhiteSpace(userId) && c.UserId == userId) ||
                    (anonymousId != null && c.AnonymousId == anonymousId))
                .Select(c => new CartResponse
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Items = c.Items
                        .Select(item => new CartItemResponse
                        {
                            Id = item.Id,
                            ProductVariantId = item.ProductVariantId,
                            ProductName = item.Product.ProductName,
                            Sku = item.Product.Sku,
                            UnitPrice = item.UnitPrice.Amount,
                            Quantity = item.Quantity,
                            LineTotal = item.TotalPrice,
                            ImageUrl = item.Product.Image == null ? null : item.Product.Image.ToString()
                        })
                        .ToList(),
                    Subtotal = c.Items.Sum(item => item.UnitPrice.Amount * item.Quantity),
                })
                .SingleOrDefaultAsync(ct);

            return cart;
        }
    }
}
