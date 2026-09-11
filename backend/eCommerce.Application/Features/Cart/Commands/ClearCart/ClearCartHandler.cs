using eCommerce.Application.Exceptions;
using eCommerce.Application.Features.Cart.DTOs;
using eCommerce.Application.Interfaces;
using eCommerce.Application.Interfaces.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Application.Features.Cart.Commands.ClearCart
{
    public class ClearCartHandler : IRequestHandler<ClearCartCommand, ReadCartDto>
    {
        private readonly IeCommerceContext _context;
        private readonly ICartQueries _queries;

        public ClearCartHandler(IeCommerceContext context, ICartQueries queries)
        {
            _context = context;
            _queries = queries;
        }

        public async Task<ReadCartDto> Handle(
            ClearCartCommand request,
            CancellationToken ct)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, ct);

            if (cart == null)
            {
                throw new NotFoundException("Cart not found.");
            }

            cart.Items.Clear();
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            var result = await _queries.GetByIdAsync(cart.Id, ct);

            if (result == null)
            {
                throw new NotFoundException(
                    "Cart could not be loaded after updating the item.");
            }

            return result;
        }
    }
}
