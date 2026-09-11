using eCommerce.Application.Features.Cart.DTOs;
using eCommerce.Application.Interfaces.Queries;
using eCommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Queries
{
    public class CartQueries : ICartQueries
    {
        private readonly eCommerceContext _context;

        public CartQueries(eCommerceContext context)
        {
            _context = context;
        }

        public async Task<ReadCartDto?> GetByIdAsync(
            int cartId,
            CancellationToken ct)
        {
            return await _context.Carts
                .AsNoTracking()
                .Where(c => c.Id == cartId)
                .Select(c => new ReadCartDto
                {
                    Id = c.Id,

                    Items = c.Items.Select(ci => new ReadCartItemDto
                    {
                        Id = ci.Id,
                        ProductVariantId = ci.ProductVariantId,

                        ProductName = _context.Products
                            .Where(p =>
                                p.Id == EF.Property<int>(ci.ProductVariant, "ProductId"))
                            .Select(p => p.Name.Value)
                            .First(),
                        OptionValues = ci.ProductVariant.OptionValues
                            .Select(ov => new ReadOptionValueFromCartDto
                            {
                                Value = ov.Value
                            })
                            .ToList(),
                        UnitPrice = ci.ProductVariant.Price!.Amount,
                        Quantity = ci.Quantity,
                        TotalPrice =
                            ci.ProductVariant.Price!.Amount * ci.Quantity
                    }).ToList(),
                    TotalPrice = c.Items.Sum(
                        i => i.ProductVariant.Price!.Amount * i.Quantity)
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
